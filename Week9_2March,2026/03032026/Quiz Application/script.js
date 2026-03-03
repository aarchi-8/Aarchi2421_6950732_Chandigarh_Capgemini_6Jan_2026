var questions = [
    {
        question: "What is 2 + 2?",
        options: ["3", "4", "5", "6"],
        correct: 1
    },
    {
        question: "Which language runs in browser?",
        options: ["C", "Java", "Python", "JavaScript"],
        correct: 3
    },
    {
        question: "HTML stands for?",
        options: [
            "Hyper Text Markup Language",
            "High Text Machine Language",
            "Hyper Tool Multi Language",
            "None"
        ],
        correct: 0
    }
];

var currentQuestion = 0;
var score = 0;
var timeLeft = 10;
var timer;

function loadQuestion() {

    clearInterval(timer);
    timeLeft = 10;
    document.getElementById("timer").innerText = "Time Left: " + timeLeft;

    timer = setInterval(function() {
        timeLeft--;
        document.getElementById("timer").innerText = "Time Left: " + timeLeft;

        if (timeLeft == 0) {
            clearInterval(timer);
            nextQuestion();
        }
    }, 1000);

    var q = questions[currentQuestion];

    document.getElementById("question").innerText = q.question;

    var buttons = document.getElementsByClassName("optionBtn");

    for (var i = 0; i < buttons.length; i++) {
        buttons[i].innerText = q.options[i];
        buttons[i].classList.remove("correct", "wrong");
    }
}

function checkAnswer(selectedIndex) {

    clearInterval(timer);

    var correctIndex = questions[currentQuestion].correct;
    var buttons = document.getElementsByClassName("optionBtn");

    if (selectedIndex == correctIndex) {
        score++;
        buttons[selectedIndex].classList.add("correct");
    } else {
        buttons[selectedIndex].classList.add("wrong");
        buttons[correctIndex].classList.add("correct");
    }
}

function nextQuestion() {

    currentQuestion++;

    if (currentQuestion < questions.length) {
        loadQuestion();
    } else {
        document.querySelector(".quiz-container").innerHTML =
            "<h2>Quiz Finished!</h2><h3>Your Score: " + score + " / " + questions.length + "</h3>";
    }
}

loadQuestion();