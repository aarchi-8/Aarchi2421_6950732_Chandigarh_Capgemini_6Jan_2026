// Course-related functions

// Load courses from API
async function loadCourses(category = "") {
  try {
    const endpoint = category
      ? `/api/v1/courses/category/${category}`
      : "/api/v1/courses";
    const response = await apiCall(endpoint);

    if (response.ok) {
      const courses = await response.json();
      displayCourses(courses);
    } else {
      showMessage("Failed to load courses", "error");
    }
  } catch (error) {
    console.error("Load courses error:", error);
    showMessage("Network error loading courses", "error");
  }
}

// Load categories for filter
async function loadCategories() {
  try {
    const response = await apiCall("/api/v1/courses");
    if (response.ok) {
      const courses = await response.json();
      const categories = [
        ...new Set(
          courses.map((course) => course.category).filter((cat) => cat),
        ),
      ];

      const categoryFilter = document.getElementById("category-filter");
      categories.forEach((category) => {
        const option = document.createElement("option");
        option.value = category;
        option.textContent = category;
        categoryFilter.appendChild(option);
      });

      categoryFilter.addEventListener("change", (e) => {
        loadCourses(e.target.value);
      });
    }
  } catch (error) {
    console.error("Load categories error:", error);
  }
}

// Display courses in grid
function displayCourses(courses) {
  const container = document.getElementById("courses-container");

  if (courses.length === 0) {
    container.innerHTML = "<p>No courses found.</p>";
    return;
  }

  container.innerHTML = courses
    .map(
      (course) => `
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">${course.title}</h3>
            </div>
            <div class="card-body">
                <p class="card-text">${course.description}</p>
                <p><strong>Category:</strong> ${course.category || "General"}</p>
                <p><strong>Instructor:</strong> ${course.instructorUsername}</p>
                <p><strong>Lessons:</strong> ${course.lessons.length}</p>
                <div style="margin-top: 1rem;">
                    <button class="btn" onclick="viewCourseDetails(${course.id})">View Details</button>
                    ${
                      currentUser && currentUser.role === "Student"
                        ? `<button class="btn btn-secondary" onclick="enrollInCourse(${course.id})">Enroll</button>`
                        : ""
                    }
                </div>
            </div>
        </div>
    `,
    )
    .join("");
}

// View course details
async function viewCourseDetails(courseId) {
  try {
    const response = await apiCall(`/api/v1/courses/${courseId}`);
    if (response.ok) {
      const course = await response.json();
      showCourseDetails(course);
    } else {
      showMessage("Failed to load course details", "error");
    }
  } catch (error) {
    console.error("View course details error:", error);
    showMessage("Network error loading course details", "error");
  }
}

// Show course details modal/popup
function showCourseDetails(course) {
  const modal = document.createElement("div");
  modal.style.cssText = `
        position: fixed; top: 0; left: 0; width: 100%; height: 100%;
        background: rgba(0,0,0,0.6); backdrop-filter: blur(5px);
        display: flex; align-items: center; justify-content: center;
        z-index: 1000; animation: fadeIn 0.3s ease-out;
    `;

  modal.innerHTML = `
        <div style="
            background: #ffffff;
            padding: 2.5rem; border-radius: 12px; max-width: 700px;
            max-height: 85vh; overflow-y: auto; box-shadow: 0 10px 25px rgba(0,0,0,0.1);
            border: 1px solid #e5e7eb; position: relative;
        ">
            <button onclick="this.closest('div').parentElement.remove()"
                style="
                    position: absolute; top: 1rem; right: 1rem;
                    background: none; border: none; font-size: 1.5rem;
                    cursor: pointer; color: #6b7280; padding: 0.5rem;
                    border-radius: 6px; transition: all 0.2s ease;
                "
                onmouseover="this.style.background='#f9fafb'"
                onmouseout="this.style.background='none'"
            >×</button>

            <h2 style="color: #374151; margin-bottom: 1rem; font-size: 2rem; font-weight: 700;">${course.title}</h2>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 2rem;">
                <div style="background: #f9fafb; padding: 1rem; border-radius: 8px; border: 1px solid #e5e7eb;">
                    <strong style="color: #374151;">Category:</strong> ${course.category || "General"}
                </div>
                <div style="background: #f9fafb; padding: 1rem; border-radius: 8px; border: 1px solid #e5e7eb;">
                    <strong style="color: #374151;">Instructor:</strong> ${course.instructorUsername}
                </div>
            </div>

            <div style="margin-bottom: 2rem;">
                <h3 style="color: #6b7280; margin-bottom: 1rem; font-size: 1.2rem;">Description</h3>
                <p style="color: #374151; line-height: 1.6; font-size: 1rem;">${course.description}</p>
            </div>

            <h3 style="color: #6b7280; margin-bottom: 1rem; font-size: 1.4rem;">Lessons (${course.lessons.length})</h3>
            ${
              course.lessons.length > 0
                ? course.lessons
                    .map(
                      (lesson, index) => `
                    <div style="
                        border: 1px solid #e5e7eb; padding: 1.5rem; margin: 0.5rem 0;
                        border-radius: 8px; background: #ffffff;
                        transition: all 0.2s ease;
                    "
                    onmouseover="this.style.background='#f9fafb'; this.style.borderColor='#d1d5db'"
                    onmouseout="this.style.background='#ffffff'; this.style.borderColor='#e5e7eb'">
                        <h4 style="color: #374151; margin-bottom: 0.5rem; font-size: 1.1rem;">
                            ${index + 1}. ${lesson.title}
                        </h4>
                        <p style="color: #6b7280; line-height: 1.6;">${lesson.content}</p>
                    </div>
                `,
                    )
                    .join("")
                : "<div style='text-align: center; padding: 2rem; color: #9ca3af; font-style: italic;'>No lessons available yet.</div>"
            }

            <div style="margin-top: 2rem; display: flex; gap: 1rem; justify-content: flex-end;">
                <button class="btn btn-secondary" onclick="this.closest('div').parentElement.remove()">Close</button>
                ${
                  currentUser && currentUser.role === "Student"
                    ? `<button class="btn" onclick="enrollInCourse(${course.id}); this.closest('div').parentElement.remove()">Enroll Now</button>`
                    : ""
                }
                ${
                  currentUser &&
                  (currentUser.role === "Instructor" ||
                    currentUser.role === "Admin") &&
                  currentUser.id === course.instructorId
                    ? `<button class="btn" onclick="addLessonToCourse(${course.id}); this.closest('div').parentElement.remove()">Add Lesson</button>`
                    : ""
                }
            </div>
        </div>
    `;

  document.body.appendChild(modal);
}

// Enroll in course
async function enrollInCourse(courseId) {
  try {
    const response = await apiCall("/api/v1/enroll", {
      method: "POST",
      body: JSON.stringify({ courseId }),
    });

    if (response.ok) {
      showMessage("Successfully enrolled in course!", "success");
      // Refresh courses to show updated enrollment status
      loadCourses();
    } else {
      const error = await response.json();
      showMessage(error.title || "Failed to enroll", "error");
    }
  } catch (error) {
    console.error("Enroll error:", error);
    showMessage("Network error during enrollment", "error");
  }
}

// Handle create course form submission
async function handleCreateCourse(e) {
  e.preventDefault();

  const submitButton = e.target.querySelector('button[type="submit"]');
  const title = document.getElementById("course-title").value;
  const description = document.getElementById("course-description").value;
  const category = document.getElementById("course-category").value;

  // Client-side validation
  if (!title || !description) {
    showMessage("Please fill in all required fields", "error");
    return;
  }

  showLoading(submitButton, "Creating course...");

  try {
    const response = await apiCall("/api/v1/courses", {
      method: "POST",
      body: JSON.stringify({ title, description, category }),
    });

    if (response.ok) {
      showMessage("Course created successfully!", "success");
      loadCoursesPage();
    } else {
      const error = await response.json();
      showMessage(error.title || "Failed to create course", "error");
    }
  } catch (error) {
    console.error("Create course error:", error);
    showMessage("Network error creating course", "error");
  } finally {
    hideLoading(submitButton);
  }
}

// Add lesson to course
function addLessonToCourse(courseId) {
  const modal = document.createElement("div");
  modal.style.cssText = `
        position: fixed; top: 0; left: 0; width: 100%; height: 100%;
        background: rgba(0,0,0,0.6); backdrop-filter: blur(5px);
        display: flex; align-items: center; justify-content: center;
        z-index: 1000; animation: fadeIn 0.3s ease-out;
    `;

  modal.innerHTML = `
        <div style="
            background: #ffffff;
            padding: 2.5rem; border-radius: 12px; max-width: 500px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.1);
            border: 1px solid #e5e7eb; position: relative;
        ">
            <button onclick="this.closest('div').parentElement.remove()"
                style="
                    position: absolute; top: 1rem; right: 1rem;
                    background: none; border: none; font-size: 1.5rem;
                    cursor: pointer; color: #6b7280; padding: 0.5rem;
                    border-radius: 6px; transition: all 0.2s ease;
                "
                onmouseover="this.style.background='#f9fafb'"
                onmouseout="this.style.background='none'"
            >×</button>

            <h2 style="color: #374151; margin-bottom: 2rem; font-size: 1.8rem; font-weight: 700; text-align: center;">Add New Lesson</h2>

            <form id="add-lesson-form">
                <div class="form-group">
                    <label for="lesson-title" style="color: #6b7280; font-weight: 500;">Lesson Title:</label>
                    <input type="text" id="lesson-title" name="title" required
                        style="border: 1px solid #d1d5db; border-radius: 8px; padding: 0.875rem 1rem; font-size: 1rem; transition: all 0.2s ease; background: #ffffff; width: 100%; box-sizing: border-box;"
                        onfocus="this.style.borderColor='#3b82f6'; this.style.boxShadow='0 0 0 3px rgba(59, 130, 246, 0.1)'"
                        onblur="this.style.borderColor='#d1d5db'; this.style.boxShadow='none'">
                </div>
                <div class="form-group">
                    <label for="lesson-content" style="color: #6b7280; font-weight: 500;">Lesson Content:</label>
                    <textarea id="lesson-content" name="content" required rows="6"
                        style="border: 1px solid #d1d5db; border-radius: 8px; padding: 0.875rem 1rem; font-size: 1rem; transition: all 0.2s ease; background: #ffffff; width: 100%; box-sizing: border-box; resize: vertical;"
                        onfocus="this.style.borderColor='#3b82f6'; this.style.boxShadow='0 0 0 3px rgba(59, 130, 246, 0.1)'"
                        onblur="this.style.borderColor='#d1d5db'; this.style.boxShadow='none'"></textarea>
                </div>
                <div style="display: flex; gap: 1rem; justify-content: flex-end; margin-top: 2rem;">
                    <button type="button" class="btn btn-secondary" onclick="this.closest('div').parentElement.remove()">Cancel</button>
                    <button type="submit" class="btn">Add Lesson</button>
                </div>
            </form>
        </div>
    `;

  document.body.appendChild(modal);

  document
    .getElementById("add-lesson-form")
    .addEventListener("submit", async (e) => {
      e.preventDefault();

      const title = document.getElementById("lesson-title").value;
      const content = document.getElementById("lesson-content").value;

      if (!title || !content) {
        showMessage("Please fill in all fields", "error");
        return;
      }

      try {
        const response = await apiCall(`/api/v1/courses/${courseId}/lessons`, {
          method: "POST",
          body: JSON.stringify({ title, content }),
        });

        if (response.ok) {
          showMessage("Lesson added successfully!", "success");
          modal.remove();
        } else {
          const error = await response.json();
          showMessage(error.title || "Failed to add lesson", "error");
        }
      } catch (error) {
        console.error("Add lesson error:", error);
        showMessage("Network error adding lesson", "error");
      }
    });
}
