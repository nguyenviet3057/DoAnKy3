const StatusCode = {
    Success: 1,
    Error: 0,
    NotFound: 2,
    Unauthorized: 3
};

const StatusAlert = {
    Error: 0,
    Success: 1,
    Warning: 2
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

function AppendAlert(message, status) {
    if ($("#alert-container")) {
        switch (status) {
            case StatusAlert.Error:
                $("#alert-container").append('<div class="mb-3 alert alert-left alert-danger alert-dismissible fade show" role="alert"><span>' + message + '</span><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                break;
            case StatusAlert.Success:
                $("#alert-container").append('<div class="mb-3 alert alert-left alert-success alert-dismissible fade show" role="alert"><span>' + message + '</span><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                break;
            case StatusAlert.Warning:
                $("#alert-container").append('<div class="mb-3 alert alert-left alert-warning alert-dismissible fade show" role="alert"><span>' + message + '</span><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
                break;
        }
    }
}
