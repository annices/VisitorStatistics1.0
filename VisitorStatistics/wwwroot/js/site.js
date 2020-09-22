/*
    Created: 2020
    Author: Annice Strömberg
*/

/**
 * This JavaScript (with Ajax and jQuery) enables partial page reloads for enhanced performance on
 * form submits, among other events for general enhanced user experience.
*/
$(document).ready(function () {

    // Set milliseconds for fade out effect on feedback messages:
    var duration = 3000;

    /**
    * Handle save event for admin settings (panel one).
    */
    $("#adminForm").on("click", function () {

        var formData = new FormData();
        formData.append("AdminID", $("#adminID").val());
        formData.append("Firstname", $("#firstName").val());
        formData.append("Lastname", $("#lastName").val());
        formData.append("Email", $("#email").val());
        formData.append("Password", $("#password").val());

        // Refresh feedback DIV on every post request to be able to show feedback again on next save:
        $("#adminFeedbackBox").load(" #adminFeedback");

        if (validInput()) {
            // Handle post request partially:
            $.ajax(
                {
                    type: 'POST',
                    cache: false,
                    contentType: false,
                    processData: false,
                    url: '/Admin/AdminSettings', // Define the endpoint.
                    data: formData,
                    success: function (formData, statusText, xhr) {
                        var feedback = $("#adminFeedbackBox");
                        feedback.find("#adminFeedback").text("The admin settings are now saved.").fadeOut(duration);
                        console.log(statusText); console.log(xhr.status); console.log(formData);
                    }
                });
        }
    });

    /**
    * Handle save event for application settings (panel two).
    */
    $("#appForm").on("click", function () {

        var formData = new FormData();
        formData.append("AppID", $("#appID").val());
        formData.append("ApplicationName", $("#appName").val());

        var urls = [];
        $.each($("input[name='ApplicationURL']"), function () {
            urls.push($(this).val());
        });
        formData.append("ApplicationURL", urls);

        $("#appFeedbackBox").load(" #appFeedback");

        if (validInput()) {

            $.ajax(
                {
                    type: 'POST',
                    cache: false,
                    contentType: false,
                    processData: false,
                    url: '/Admin/AppSettings',
                    data: formData,
                    success: function (formData, statusText, xhr) {
                        var feedback = $("#appFeedbackBox");
                        feedback.find("#appFeedback").text("The application settings are now saved.").fadeOut(duration);
                        console.log(statusText); console.log(xhr.status); console.log(formData);
                    }
                })
        }
    });

    /**
     * Handle save event for visitor settings (panel three).
     */
    $("#ipForm").on("click", function () {

        var formData = new FormData();
        formData.append("AppID", $("#applID").val());

        var ips = [];
        $.each($("input[name='IPAddress']"), function () {
            ips.push($.trim($(this).val()));
        });
        formData.append("IPAddress", ips);

        formData.append("DaysBeforeDeletion", $("#daysBeforeDeletion").val());

        $("#visitFeedbackBox").load(" #visitFeedback");

        if (validInput()) {

            $.ajax(
                {
                    type: 'POST',
                    cache: false,
                    contentType: false,
                    processData: false,
                    url: '/Admin/VisitSettings',
                    data: formData,
                    success: function (formData, statusText, xhr) {
                        var feedback = $("#visitFeedbackBox");
                        feedback.find("#visitFeedback").text("The visit settings are now saved.").fadeOut(duration);
                        console.log(statusText); console.log(xhr.status); console.log(formData);
                    }
                })
        }
    });

    // Create feedback to be displayed in tables when no URL and/or IP rows exist:
    var rowCount = $('#urlTable').find('td').length;
    var noUrlFeedback = "No URLs are registered for the application.";
    if (rowCount == 0) {
        $('#urlBody').append("<tr><td><em>" + noUrlFeedback + "</em></td></tr>");
    }
    var noIpFeedback = "No IP addresses are ignored.";
    var rowCount = $('#ipTable').find('td').length;
    if (rowCount == 0) {
        $('#ipBody').append("<tr><td><em>" + noIpFeedback + "</em></td></tr>");
    }

    /**
     * Handle click event to add URLs to be registered for the application (edit mode before DB save).
     */
    $(".add-url").on("click", function () {
        var rowID = generateID(".plaintext");
        var url = "#url";

        if ($.trim($(url).val()) == '') {

            $(url).prop("class", "mandatory");
            $(url).prop("placeholder", "Please type a URL...");
        }
        else if ($(url).val().indexOf(",") > -1) {
            $(url).prop("class", "mandatory");
            $(url).prop("value", ""); // Clear value to be able to show placeholder msg.
            $(url).prop("placeholder", "The URL cannot contain commas...");
        }
        else {
            $(url).removeClass("mandatory");
            // Remove the no rows message when the first IP row is added:
            deleteTableRowByContent("urlTable", noUrlFeedback);

            // Prepare the URL row to be added to the table body:
            var urlMarkup = "<tr class='tr-row'><td style='width: 10px; padding-left: 15px' class='url-and-ip-rows'>" +
                "<input type='checkbox' name='urlRecord' />" +
                "</td><td class='url-and-ip-rows'>" +
                "<input type='text' id='" + rowID + "' class='plaintext' value='" + $(url).val() + "' name='ApplicationURL'" +
                "onblur='handleAttributes(" + rowID + ")' disabled /> " +
                "</td><td style='text-align: right'>" +
                "<a href='#' onclick='focusElement(" + rowID + ")' style='color: #000' title='Edit URL.'><i class='fa fa-edit'></i></a>" +
                " &nbsp;&nbsp; " +
                "<a href='#' onclick='$(this).closest(\"tr\").remove()' style='font-weight: bold; color: #000' title='Remove URL.'><i class='fa fa-trash'></i></a>" +
                "</td></tr>";

            $("#urlBody").append(urlMarkup);
        }
    });

    /**
     * Handle click event to add IP addresses to be ignored for the application (edit mode before DB save).
     */
    $(".add-ip").on("click", function () {
        var rowID = generateID(".plaintext");
        var ip = "#ip";

        if ($.trim($(ip).val()) == '') {
            $(ip).prop("class", "mandatory");
            $(ip).prop("placeholder", "Please type an IP address...");
        }
        else if ($(ip).val().indexOf(",") > -1) {
            $(ip).prop("class", "mandatory");
            $(ip).prop("value", ""); // Clear value to be able to show placeholder msg.
            $(ip).prop("placeholder", "The IP address cannot contain commas...");
        }
        else {
            $(ip).removeClass("mandatory");
            // Remove the no rows message when the first IP row is added:
            deleteTableRowByContent("ipTable", noIpFeedback);

            // Prepare the IP row to be added to the table body:
            var ipMarkup = "<tr class='tr-row'><td style='width: 10px; padding-left: 15px' class='url-and-ip-rows'>" +
                "<input type='checkbox' name='ipRecord' />" +
                "</td><td class='url-and-ip-rows'>" +
                "<input type='text' id='" + rowID + "' class='plaintext' value='" + $(ip).val() + "' name='IPAddress'" +
                "onblur='handleAttributes(" + rowID + ")' disabled /> " +
                "</td><td style='text-align: right'>" +
                "<a href='#' onclick='focusElement(" + rowID + ")' style='color: #000' title='Edit IP.'><i class='fa fa-edit'></i></a>" +
                " &nbsp;&nbsp; " +
                "<a href='#' onclick='$(this).closest(\"tr\").remove()' style='font-weight: bold; color: #000' title='Remove IP.'><i class='fa fa-trash'></i></a>" +
                "</td></tr>";

            $("#ipBody").append(ipMarkup);
        }
    });

    /**
     * Handle remove click event on multi-selection.
     */
    $(".delete-row").on("click", function () {
        $("table tbody").find('input[name="urlRecord"]').each(function () {
            if ($(this).is(":checked")) {
                $(this).parents("tr").remove();
            }
        });

        $("table tbody").find('input[name="ipRecord"]').each(function () {
            if ($(this).is(":checked")) {
                $(this).parents("tr").remove();
            }
        });
    });

    /**
     * Handle focus out/blur function on existing (stored in DB) URLs and IP addresses.
     */
    $(".plaintext").blur(function () {

        // If the value does NOT contain commas:
        if ($(this).val().indexOf(",") == -1) {
            $(this).removeClass("mandatory");
            $(this).prop("disabled", true);
        }
        else {
            $(this).prop("disabled", false);
        }
    });

}); // End document ready function.

/**
 * Global method to handle attributes of an HTML element by calling its ID. This
 * method is used to handle attributes on blur events of input values that are still in
 * edit mode, i.e. URLs and IP addresses that are not yet saved to DB.
 * @param {any} id
 * @param {any} attribute
 * @param {any} value
 */
handleAttributes = function attribute(id) {

    var elementID = document.getElementById(id);

    if ($("#" + id).val().indexOf(",") == -1) {
        $("#" + id).removeClass("mandatory");
        elementID.setAttribute("disabled", false);
    }
    else {
        elementID.setAttribute("disabled", true);
    }
}

/**
 * Method to handle input logic on the client side for enhanced performance.
 */
function validInput() {

    // Specify inputs to check:
    var email = "#email";
    var appName = "#appName";
    var delDays = "#daysBeforeDeletion";
    var notValidUrl = isContentInTable("urlTable", "plaintext", ",");
    var notValidIP = isContentInTable("ipTable", "plaintext", ",");

    // Check admin settings input:
    if ($.trim($(email).val()) == '' || !isEmail($(email).val())) {
        $(email).addClass("mandatory");
        $(email).prop("value", ""); // Clear value to be able to show placeholder msg.
        $(email).prop("placeholder", "Please type a valid email...");

        return false;
    }
    // Check application settings input:
    else if ($.trim($(appName).val()) == '') {
        $(appName).addClass("mandatory");
        $(appName).prop("value", "");
        $(appName).prop("placeholder", "Please type an application name...");

        return false;
    }
    else if (notValidUrl.status == true) {
        var urlID = notValidUrl.msg;
        $("#" + urlID).addClass("plaintext mandatory");
        $("#" + urlID).prop("disabled", false);

        if ($('#url-input-error').length == 0) {
            $(".url-feedback").append('<span id="url-input-error" class="errormsg">The URL cannot contain commas.</span>');
        }

        return false;
    }
    else if (notValidIP.status == true) {
        var ipID = notValidIP.msg;
        $("#" + ipID).addClass("plaintext mandatory");
        $("#" + ipID).prop("disabled", false);

        if ($('#ip-input-error').length == 0) {
            $(".ip-feedback").append('<span id="ip-input-error" class="errormsg">The IP address cannot contain commas.</span>');
        }

        return false;
    }
    // Check deletion days:
    if ($(delDays).val() < 1) {
        $(email).addClass("mandatory");

        return false;
    }
    else {
        $(email).removeClass("mandatory");
        $(appName).removeClass("mandatory");
        $("#url-input-error").remove();
        $("#url-feedback").remove();
        $("#ip-input-error").remove();
        $("#ip-feedback").remove();

        return true;
    }
}

/**
 * Method to check for specific element content in a table.
 * @param {any} tableID
 * @param {any} className
 * @param {any} content
 */
function isContentInTable(tableID, className, content) {

    var table = document.getElementById(tableID);
    var elements = table.getElementsByClassName(className);
    var response = { status: false, msg: '' };

    for (var i = 0; i < elements.length; i++) {

        if (elements[i].value.indexOf(content) > -1) {
            response.msg = elements[i].id;
            response.status = true;
            return response;
        }
    }

    return response;
}

/**
 * Method to remove a class from an input field if it has a value.
 * @param {any} id
 */
removeRedIfVal = function removeIfValue(id) {
    if (id.value != '') {
        $("#" + id).removeClass('mandatory');
    }
}

/**
 * Method to check if input is valid email format.
 * @param {any} email
 */
function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

/**
 * Method to generate a temporary ID for each given element by checking max of existing
 * IDs and then increment the next ID by one. This is used for the added URLs and IPs
 * that have not yet been saved to DB to prevent conflicts with the DB IDs.
 * @param {any} element
 */
function generateID(element) {

    var max = 0;
    $(element).each(function () {
        max = Math.max(this.id, max);
    });

    var newId = max + 1;
    return newId;
}

/**
 * On page load, increment temporary IDs for each existing app URL read from DB.
 * This is to prevent conflicts with the temporary IDs generated for added app URLs
 * in edit mode, i.e. the ones not yet saved to DB.
 */
window.onload = function generateIdOnPageLoad() {

    var id = 1;

    $("#urlTable tr").each(function () {

        $('.plaintext', this).each(function () {
            $(this).prop("id", id);
            id++;
        });
    });

    $("#ipTable tr").each(function () {

        $('.plaintext', this).each(function () {
            $(this).prop("id", id);
            id++;
        });
    });
}


/**
* Method to search for specific content within a specific HTML table cell.
* Once there's a match, the table row and its content will be removed.
* @param {any} tableID
* @param {any} content
*/
function deleteTableRowByContent(tableID, content) {

    var table = document.getElementById(tableID);

    for (var r = 0, n = table.rows.length; r < n; r++) {
        for (var c = 0, m = table.rows[r].cells.length; c < m; c++) {

            if (table.rows[r].cells[c].innerHTML.indexOf(content) > -1) {
                table.deleteRow(r);
            }
        }
    }
}

/**
* Method to handle the appearance of the dropdown site menu on triggered responsive
* design by calling different CSS classes on different conditions.
*/
function dropDown() {
    var element = document.getElementById("myTopnav");
    if (element.className === "topnav") {
        element.className += " responsive";
    } else {
        element.className = "topnav";
    }
}

/**
 * Global method to select a checkbox by its ID and in turn trigger 
 * a multi-select function of other checkboxes by their input names.
 * @param {any} id
 * @param {any} element
 */
multiSelect = function selectAll(id, element) {

    if ($("#" + id).is(":checked")) {
        $(element).each(function () {
            $(this).prop("checked", true);
        });
    }
    else {
        $(element).each(function () {
            $(this).prop("checked", false);
        });
    }
}

/**
 * Global method to find an HTML element by its ID, then remove the 
 * "disabled" attribute from it and set the element as focused.
 * @param {any} id
 */
focusElement = function setFocus(id) {
    var id = document.getElementById(id);
    id.removeAttribute("disabled");
    id.focus();
}