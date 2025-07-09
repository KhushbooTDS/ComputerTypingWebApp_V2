// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function UpdateStudentRecord() {
    var formData = $(this).serialize();
    $.ajax({
        url: '/Admin/UpdateStudent',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                alert('Student details updated successfully!');
            } else {
                alert('Error updating student details.');
            }
        },
        error: function () {
            alert('Something went wrong. Please try again later.');
        }
    });
}

function EditStudent(studentId) {
    $.ajax({
        url: '/Admin/EditStudent',
        type: 'GET',
        data: { id: studentId },
        success: function (response) {
            if (response && response.length > 0) {
                const student = response[0];
                $("#EditstudentName").val(student.firstName + ' ' + student.lastName || 'N/A');
                $("#EditmobileNo").val(student.mobileNumber || 'N/A');
                $("#Editgender").val(student.gender || 'N/A');
                $("#Edithandicap").val(student.handicap || 'N/A');
                $("#Editschool").val(student.school || 'N/A');
                $("#EditdateAdded").val(student.dateAdd || 'N/A');
                if (student.selectSub30wpm != null && student.selectSub30wpm != '') {
                    var sub30 = student.selectSub30wpm.split(',');
                    for (var i = 0; i < sub30.length; i++) {
                        $(`#${sub30[i]}`).prop('checked',true)
                    }
                }
                if (student.selectSub40wpm != null && student.selectSub40wpm != '') {
                    var sub40 = student.selectSub40wpm.split(',');
                    for (var i = 0; i < sub40.length; i++) {
                        $(`#${sub40[i]}`).prop('checked', true)
                    }
                }
                const modal = new bootstrap.Modal(document.getElementById('EditstudentModal'));
                modal.show();
            }
        }
    });
}


function ViewStudent(studentId) {
    $.ajax({
        url: '/Admin/EditStudent',
        type: 'GET',
        data: { id: studentId },
        success: function (response) {
            if (response && response.length > 0) {
                const student = response[0];
                $("#StudentNameDisplay").text(student.firstName + ' ' + student.lastName || 'N/A');
                $("#MobileNoDisplay").text(student.mobileNumber || 'N/A');
                $("#GenderDisplay").text(student.gender || 'N/A');
                $("#HandicapDisplay").text(student.handicap || 'N/A');
                $("#SchoolDisplay").text(student.school || 'N/A');
                $("#DateAddDisplay").text(student.dateAdd || 'N/A');

                $("#StudentSub30Display").text(student.selectSub30wpm);
                $("#StudentSub40Display").text(student.selectSub40wpm);

                const modal = new bootstrap.Modal(document.getElementById('studentModal'));
                modal.show();
            } else {
                alert("No student data found in the response.");
            }
        },
        error: function () {
            alert("An error occurred while fetching the student data.");
        }
    });
}

function DeleteStudent(studentId) {
    if (confirm("Are you sure you want to delete this student?")) {
        $.ajax({
            url: '/Admin/DeleteStudent',
            type: 'POST',
            data: { id: studentId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    location.reload(true);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("An error occurred while deleting the student.");
            }
        });
    }
}

//document.getElementById("courseDropdown").addEventListener("change", function () {
//    var courseId = this.value;
//    if (courseId) {
//        fetch(`/SuperAdmin/GetSubjectsByCourseId?courseId=${courseId}`)
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error("Network response was not ok");
//                }
//                return response.json();
//            })
//            .then(data => {
//                $("#subjectDropdown").empty();
//                $("#subjectDropdown").append('<option value="">Select Subject</option>');
//                data.forEach(item => {
//                    $("#subjectDropdown").append(`<option value="${item.subjectName}">${item.subjectName}</option>`);
//                });
//            })
//            .catch(error => console.error("Error fetching subjects:", error));
//    } else {
//        console.log("No course selected.");
//        $("#subjectDropdown").empty();
//        $("#subjectDropdown").append('<option value="">Select Subject</option>');
//    }
//});

function updateLanguage() {
    var dropdown = document.getElementById("subjectDropdown");
    var selectedValue = dropdown.value;
    const textarea = document.getElementById('textarea');

    textarea.classList.remove('krutidev', 'marathi');

    if (selectedValue == "2" || selectedValue == "5") {
        textarea.classList.add('krutidev');
        textarea.classList.remove('marathi');

    } else if (selectedValue == "3" || selectedValue == "6") {
        textarea.classList.add('marathi');
        textarea.classList.remove('krutidev');
    }
}


