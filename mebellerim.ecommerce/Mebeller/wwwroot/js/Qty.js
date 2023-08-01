$(".increase").click(function () {
    if (parseInt($(this).prev().val()) < parseInt($(this).prev().prev().val())) {
        $(this).prev().val(parseInt($(this).prev().val()) + 1);
    }
});
$(".reduced").click(function () {
    if (parseInt($(this).prev().prev().val()) > 1) {
        $(this).prev().prev().val(parseInt($(this).prev().prev().val()) - 1);
    }
});
$(".items-count").click(function () {
    const input = $(this).closest('.product-count').find('.qty');
    const thisValue = parseInt(input.val());
    const maxQuantity = parseInt(input.prev().val());

    if ($(this).hasClass('reduced') && thisValue > 1) {
        input.val(thisValue - 1);
    }
    else if ($(this).hasClass('increase') && thisValue < maxQuantity) {
        input.val(thisValue + 1);
    }
});

$(".update-cart-button").click(function () {
    const quantities = $(".order-details-quantities");

    for (let i = 0; i < quantities.length; i++) {
        $("<input type='hidden' name='orderDetailsQuantities' value='" + $(quantities[i]).val() + "' />").insertBefore(this);
    }

    $("#update-cart-form").submit();
});
$(".proceed-to-checkout-button").click(function () {
    const quantities = $(".order-details-quantities");

    for (let i = 0; i < quantities.length; i++) {
        $("<input type='hidden' name='orderDetailsQuantities' value='" + $(quantities[i]).val() + "' />").insertBefore(this);
    }

    $("#proceed-to-checkout-form").submit();
});