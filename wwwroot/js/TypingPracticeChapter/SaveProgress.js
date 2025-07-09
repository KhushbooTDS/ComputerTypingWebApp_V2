
$(document).ready(function () {
    $('#timer').text('1:00')
})
// Function to call when timer ends
function timerFinished() {
    console.log("Time's up! Calling the function...");
    // Your desired action here
}
function timerFinished() {
    const timerDiv = document.getElementById("timer");
    Save();
    // Additional actions can go here
}
// Start a 7-minute timer
function startTimer(durationMinutes) {
    const timerDiv = document.getElementById("timer");
    let duration = durationMinutes * 60; // Convert minutes to seconds

    const timerInterval = setInterval(() => {
        let minutes = Math.floor(duration / 60); // Get remaining minutes
        let seconds = duration % 60; // Get remaining seconds

        // Update the div with minutes and seconds
        timerDiv.innerText = `${minutes}:${seconds.toString().padStart(2, '0')}`;

        // Check if the timer has reached 0
        if (duration <= 0) {
            clearInterval(timerInterval);
            //timerFinished();
            Save();
        }
        duration--; // Decrement the duration
    }, 1000); // Update every second
}
function Save() {
    var data = {
        StudentId: 1,
        PracticeId: $('#PracticeId').val(),
        SubjectId: $('#SubjectId').val(),
        TotalCorrectCharacters: document.querySelectorAll('.text-good').length,
        TotalIncorrectCharacters: document.querySelectorAll('.text-error').length
    };
    $.ajax({
        url: '/Student/SaveProgress',
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (data) {
            
            if (data != null) {
                $('.typingBody').hide();
                $('.resultBody').removeClass('d-none')
                $('#handGesture').hide();
                $('#studentName').text(data.userName)
                $('#grossSpeed').text(data.grossSpeedPerMinute)
                $('#netSpeed').text(data.netSpeedPerMinute)
                $('#correctWord').text(data.totalCorrectCharacters)
                $('#incorrectWord').text(data.totalIncorrectCharacters)
                $('#accuracy').text(Math.round(data.netSpeedPerMinute * 100 / data.grossSpeedPerMinute) + "%")
            }
        },
        error: function (xhr, status, error) {
            console.error('Error: ', error);
        }
    });
}
