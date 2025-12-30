console.log("Contact form script loaded");

$(document).ready(function () {
    // Form submit handle karo
    $("#contactForm").on("submit", function (e) {
        e.preventDefault(); // Page refresh rok do

        console.log("Form submit clicked");

        // Form values lo
        let firstName = $("#firstName").val().trim();
        let lastName = $("#lastName").val().trim();
        let email = $("#email").val().trim();
        let subject = $("#subject").val();
        let message = $("#message").val().trim();

        // ========== VALIDATION WITH ALERTS ==========

        // Check if any field is empty
        if (!firstName || !lastName || !email || !subject || !message) {
            alert("⚠️ Please fill in ALL required fields!");
            return; // Stop here
        }

        // Check email format
        let emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            alert("📧 Please enter a VALID email address!");
            return; // Stop here
        }

        // ========== ALL VALIDATION PASSED ==========

        // Data prepare karo
        let formData = {
            FirstName: firstName,
            LastName: lastName,
            Email: email,
            Subject: subject,
            Message: message
        };

        $("#sendBtn").prop("disabled", true).html("Sending...");  // to prevent multiple send message requests

        console.log("Sending data:", formData);

        // AJAX request bhejo
        $.ajax({
            url: "/Home/ContactAjax",
            type: "POST",
            data: formData,
            success: function (response) {
                console.log("Server response:", response);

                if (response.success) {
                    // SUCCESS ALERT
                    alert("✅ Your message has been sent successfully!");

                    // Form clear karo
                    $("#contactForm")[0].reset();

                    // Optional: Page par bhi message dikhao
                    $("#formMessage").html(
                        `<div class="alert alert-success">
                            ✅ Your message has been sent successfully!
                        </div>`
                    );
                }
                else {
                    // ERROR ALERT from server
                    alert("❌ Error: " + response.message);

                    // Optional: Page par bhi dikhao
                    $("#formMessage").html(
                        `<div class="alert alert-danger">
                            ❌ ${response.message}
                        </div>`
                    );
                }

                $("#sendBtn").prop("disabled", false).html("Send Message");  // btn again activated from disabled
            },
            error: function (xhr, status, error) {

                // AJAX ERROR ALERT
                alert("❌ Server error! Please try again later.");

                // Optional: Page par bhi dikhao
                $("#formMessage").html(
                    `<div class="alert alert-danger">
                        ❌ Server error occurred. Please try again.
                    </div>`
                );

                $("#sendBtn").prop("disabled", false).html("Send Message");
            }
        });
    });
});