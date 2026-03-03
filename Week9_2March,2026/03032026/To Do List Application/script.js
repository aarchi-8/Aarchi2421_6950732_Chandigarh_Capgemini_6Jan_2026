// Load tasks when page loads
window.onload = function() {
    loadTasks();
};

function addTask() {

    var input = document.getElementById("taskInput");
    var taskText = input.value;

    if (taskText == "") {
        alert("Please enter a task");
        return;
    }

    createTaskElement(taskText, false);

    saveTask(taskText, false);

    input.value = "";
}

function createTaskElement(taskText, isCompleted) {

    var li = document.createElement("li");

    var span = document.createElement("span");
    span.innerText = taskText;

    if (isCompleted) {
        span.classList.add("completed");
    }

    span.onclick = function() {
        span.classList.toggle("completed");
        updateStorage();
    };

    var deleteBtn = document.createElement("button");
    deleteBtn.innerText = "Delete";
    deleteBtn.className = "deleteBtn";

    deleteBtn.onclick = function() {
        li.remove();
        updateStorage();
    };

    li.appendChild(span);
    li.appendChild(deleteBtn);

    document.getElementById("taskList").appendChild(li);
}

function saveTask(taskText, isCompleted) {

    var tasks = JSON.parse(localStorage.getItem("tasks")) || [];

    tasks.push({ text: taskText, completed: isCompleted });

    localStorage.setItem("tasks", JSON.stringify(tasks));
}

function loadTasks() {

    var tasks = JSON.parse(localStorage.getItem("tasks")) || [];

    for (var i = 0; i < tasks.length; i++) {
        createTaskElement(tasks[i].text, tasks[i].completed);
    }
}

function updateStorage() {

    var listItems = document.querySelectorAll("#taskList li");
    var tasks = [];

    listItems.forEach(function(li) {
        var text = li.querySelector("span").innerText;
        var completed = li.querySelector("span").classList.contains("completed");

        tasks.push({ text: text, completed: completed });
    });

    localStorage.setItem("tasks", JSON.stringify(tasks));
}