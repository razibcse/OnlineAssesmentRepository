
GetData = () => {
    //console.log('start');
    var token = localStorage.getItem("token");

    $.ajax({
        type: 'GET',
        url: 'https://localhost:44360/api/posts',
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log(res);
            $.each(res, function (index, value) {
                addRow(value.id,value.title, value.description,value.isChangeable);
            });
        }
    })
}

GetTrashedData = () => {
    //console.log('start');
    var token = localStorage.getItem("token");

    $.ajax({
        type: 'GET',
        url: 'https://localhost:44360/api/posts/trashed-posts',
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log('trasg oist=='+res);
            $.each(res, function (index, value) {
                addRowTrashed(value.id, value.title, value.description, value.isChangeable);
            });
        }
    })
}

GetTrashedData();

addRowTrashed = (id, title, description, ischangeable) => {
    $('#trashedpostlistbody').append('<tr><td>' + title + '</td>' + '<td>' +
        description + '</td><td>' + '</td>' + '<td><button onclick="RecoverPost(\'' + id + '\')" class="btn btn-danger"> Recover</button> </td ></tr > ');

}


RecoverPost = (id) => {
    var token = localStorage.getItem("token");

    $.ajax({
        type: 'GET',
        url: `https://localhost:44360/api/posts/recover/${id}`,
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log(res);
            window.location.replace("https://localhost:44372/");
        }
    })
}


GetUserList = () => {
    //console.log('start  fetching users');
    var token = localStorage.getItem("token");
   // console.log(token)
    $.ajax({
        type: 'GET',
        url: 'https://localhost:44360/api/userManagement/users',
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log(res);
            $.each(res, function (index, value) {
                addRowUser(value.id, value.firstName, value.lastName, value.email);
            });
            // window.location.replace("https://localhost:44372/userManagement/users");
        }
    })
}





GetTrashedUserList = () => {
    //console.log('start  fetching users');
    var token = localStorage.getItem("token");
    // console.log(token)
    $.ajax({
        type: 'GET',
        url: 'https://localhost:44360/api/userManagement/t-users',
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log(res);
            $.each(res, function (index, value) {
                addRowTrashedUser(value.id, value.firstName, value.lastName, value.email);
            });
            // window.location.replace("https://localhost:44372/userManagement/users");
        }
    })
}

GetTrashedUserList();

addRowTrashedUser = (id, firstName, lastName, email) => {
    console.log(id)
    $('#trasheduserlistbody').append('<tr><td>' + firstName + '</td>' + '<td>' +
        lastName + '</td><td>' + email + '</td>' + '<td><button onclick="RecoverUser(\'' + id + '\')" class="btn btn-danger"> Recover</button> </td ></tr > ');

}

RecoverUser = (id) => {
    var token = localStorage.getItem("token");

    $.ajax({
        type: 'GET',
        url: `https://localhost:44360/api/userManagement/t-user/${id}`,
        headers: {
            Authorization: 'Bearer ' + token
        },
        success: function (res) {
            console.log(res);
            window.location.replace("https://localhost:44372/");
        }
    })
}

GetUserList();
// ' + modalButtonUpdateUser(id) + modalBodyUser(id, firstName, lastName, email) + '
addRowUser = (id, firstName, lastName, email) => {
    console.log(id)
    $('#userlistbody').append('<tr><td>' + firstName + '</td>' + '<td>' +
        lastName + '</td><td>'+email+'</td>' + '<td><button onclick="DeleteUser(\''+id+'\')" class="btn btn-danger"> Delete</button> </td ></tr > ');

}

modalButtonUpdate = (id) => {
    var action = '<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#UpdateModal'+id+'" >Update</button >';
    return action;
}

modalBody = (id, title, description, ischangeable) => {
    var check = ischangeable ? 'checked' : '';
    console.log(ischangeable);
    console.log(check);
    var modal = '<div class="modal fade" id="UpdateModal' + id + '" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true"> <div class="modal-dialog" role="document"> <div class="modal-content"> <div class="modal-header"> <h5 class="modal-title" id="exampleModalLabel">Update Post</h5> <button type="button" class="close" data-dismiss="modal" aria-label="Close"> <span aria-hidden="true">&times;</span> </button> </div> <div class="modal-body"> <form onsubmit="return jQueryAjaxPut(this);"> <input type="hidden" name="id" class="id1" value="' + id + '"/> <div class="form-group"> <label for="title">Title</label> <input type="text" name="title" class="form-control title1" value="' + title + '" /> </div> <div class="form-group"> <label for="description">Description</label> <input type="text" name="description" class="form-control description1" value="' + description +'" /> </div><button type="submit">Update</button> </form> </div> </div> </div></div>';

    return modal;
}

DeleteUser = (id) => {
    console.log(id);
    var r = confirm("Sure to trash user??!");
    if (r == true) {
        var token = localStorage.getItem("token");
        // console.log(token)
        $.ajax({
            type: 'DELETE',
            url: `https://localhost:44360/api/userManagement/delete/${id}`,
            headers: {
                Authorization: 'Bearer ' + token
            },
            success: function (res) {
                window.location.replace("https://localhost:44372/");
            }
        })
    }
}

DeletePost = (id) => {
    console.log(id);
    var r = confirm("Sure to trash it??!");
    if (r == true) {
        var token = localStorage.getItem("token");
        // console.log(token)
        $.ajax({
            type: 'DELETE',
            url: `https://localhost:44360/api/posts/${id}`,
            headers: {
                Authorization: 'Bearer ' + token
            },
            success: function (res) {
                window.location.replace("https://localhost:44372/");
            }
        })
    }
}

addRow = (id,title,description,ischangeable) => {
    $('#postsbody').append('<tr><td>' + title + '</td>' + '<td>' +
        description + '</td>' + '<td>' + modalButtonUpdate(id) + modalBody(id, title, description, ischangeable) + '|<button onclick="DeletePost(' + id +')" class="btn btn-danger"> Delete</button> </td ></tr > ');

}

GetData();

jQueryAjaxPost = form => {
    console.log('post')
    var data = {
        title: $(".title").val(),
        description: $(".description").val(),
    };
    var token = localStorage.getItem("token");
    try {
        $.ajax({
            type: 'POST',
            url: 'https://localhost:44360/api/posts',
            data: JSON.stringify(data),
            contentType: 'application/json',
            headers: {
                Authorization: 'Bearer ' + token
            },
            success: function (res) {
                console.log('sucees');
                window.location.replace("https://localhost:44372/");
            },
            error: function (err) {
                console.log(err)
            }
        })
    } catch (ex) {
        console.log(ex)
    }

    return false;
}

jQueryAjaxPut = form => {
    console.log('run');
    try {
        var data = {
            id: $(".id1").val(),
            title: $(".title1").val(),
            description: $(".description1").val()
        };
        console.log(data)
        var token = localStorage.getItem("token");
        $.ajax({
            type: 'PUT',
            url: `https://localhost:44360/api/posts/${data.id}`,
            data: JSON.stringify(data),
            contentType: 'application/json',
            headers: {
                Authorization: 'Bearer ' + token
            },
            success: function (res) {
                window.location.replace("https://localhost:44372/");
            },
            error: function (err) {
                console.log(err)
            }
        })
    } catch (ex) {
        console.log(ex)
    }

    return false;
}

AjaxRegister = form => {
    console.log('register')
    var data = {
        email: $(".email").val(),
        password: $(".password").val(),
        confirmPassword: $(".confirmpassword").val(),
        firstName: $(".fname").val(),
        lastName: $(".lname").val(),
        country: $(".country").val()
    };
    console.log(data)
    try {
        if (data.password !== data.confirmPassword) {
            console.log(data.password)
            console.log(data.confirmPassword)
            alert('Password doesnt match')
            return false;
        }
        console.log(data.password)
        console.log(data.confirmPassword)
        console.log(data.firstName)
        console.log(data.lastName)
        console.log(data.country)
        $.ajax({
            type: 'POST',
            url: 'https://localhost:44360/api/users/register',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (res) {
                console.log(res);
            },
            error: function (err) {
                console.log(err)
            }
        })
    } catch (ex) {
        console.log(ex)
    }

    return false;
}



AjaxLogin = form => {
    var d = {
        email: $(".email").val(),
        password: $(".pass").val()
    };
    try {
        $.ajax({
            type: 'POST',
            url: 'https://localhost:44360/api/users/login',
            data: JSON.stringify(d),
            contentType:'application/json',
            success: function (res) {
                console.log(res);
                console.log(res.token)
                localStorage.setItem("token",res.token)
            },
            error: function (err) {
                console.log(err)
            }
        })
    } catch (ex) {
        console.log(ex)
    }

    return false;
}