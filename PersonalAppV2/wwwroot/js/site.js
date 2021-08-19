function showToast(message) {
    $('#toastBody').html(message);
    $('#toast').toast('show')
}

$(document).ready(function () {

    $('#toast').toast('hide')
})