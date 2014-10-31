$(function () {
    $("input[type='submit']").button();

    $("input#loginButton").on("click", function (e) {
        e.preventDefault();

        var isMissing = false;

        $("input.required", "div#innerLogin").each(function () {
            if ($(this).val() == '' ||
                $(this).val() == $(this).attr('placeholder')) {

                $(this).css("background-color", "Yellow");
                isMissing = true;
            }
        });

        if (isMissing) {
            alert('Username and/or password are required.');
            e.stopImmediatePropagation();
            return;
        }
    });

    $("input#loginButton").on("click", function(e) {
        e.preventDefault();
        var userName = $("input#userName").val();
        var password = $("input#password").val();

        $.validateUser(userName, password);
    });
});

(function ($) {

    $.validateUser = function (userName, password) {
        $.ajax({
            type: "POST",
            dataType: "json",
            timeout: 10000,
            contentType: "application/json",
            url: "../Services/Login.asmx/ValidateUser",
            data: JSON.stringify({ userName: userName, password: password }),
            beforeSend: function() {
                $("input#loginButton").val("Logging in...").attr("disabled", "disabled");
            },
            success: function(data) {

                if (data.d == "Police") {
                    window.location.href = 'Index.aspx';
                } else if (data.d == "User") {
                    alert("Sorry! Web site access is restricted to police users only.");
                } else {
                    alert(data.d);
                }

                $("input#loginButton").val("Login").removeAttr("disabled");
            },
            error: function() {
                alert("Unable to communicate with the server. Please try again.");
                $("input#loginButton").val("Login").removeAttr("disabled");
            }
        });
    };
})(jQuery);