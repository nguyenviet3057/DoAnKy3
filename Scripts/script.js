const StatusCode = {
    Success: 1,
    Error: 0,
    NotFound: 2,
    Unauthorized: 3
};

function setCookie(cname, cvalue, ctime) {
    const d = new Date();
    d.setTime(d.getTime() + (ctime * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

let access_token = getCookie("access_token");
if (access_token != "") {
    $.ajax({
        url: "/login/validate",
        data: access_token,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (result) {
            switch (result.status) {
                case StatusCode.Success:
                    break;
                default:
                    setCookie("access_token", "", -1);
                    window.location.pathname = "/login";
                    break;
            }
        }
    })
} else {
    setCookie("access_token", "", -1);
    window.location.pathname = "/login";
}

$("#logout-btn").click(function () {
    $.ajax({
        url: "/login/logout",
        data: access_token,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (result) {
            switch (result.status) {
                case StatusCode.Success:
                    setCookie("access_token", "", -1);
                    window.location.pathname = "/login";
                    break;
                default:
                    $("#alert-container").append('<div class="mb-3 alert alert-left alert-danger alert-dismissible fade show" role="alert"><span>Logout failed.</br>Please contact Technical Support!</span><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                    break;
            }
        }
    })
})

$(document).ready(function () {
    let curPath = "/" + window.location.pathname.split("/")[1];
    $("div.sidebar-list li.nav-item a.nav-link.active:not(.static-item)").removeClass("active");
    $("div.sidebar-list li.nav-item a.nav-link:not(.static-item)").each(function (index, element) {
        if ($(element).attr("href").toLowerCase() == curPath.toLowerCase()) $(this).addClass("active");
    })
})
