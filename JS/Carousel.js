//carousel
var index = 0;
var imgelements = document.querySelectorAll(".carousel_img");
var carousel_btn_preview = document.getElementById("carousel_btn_preview");
var carousel_btn_next = document.getElementById("carousel_btn_next");
var selectimgelements = document.querySelectorAll(".btn_select_img");
var container = document.querySelector(".carousel_container");
var uplimit = imgelements.length - 1;
var imgwidth = imgelements[0].clientWidth;
var intervalId;

// function startAutoSlide() {
//     intervalId = setInterval(() => {
//         index = (index + 1) % imgelements.length;
//         updatecarousel();
//     }, 3000); // 轮播间隔时间为 3 秒
// }

function stopAutoSlide() {
    clearInterval(intervalId);
}

function previewimg() {
    //stopAutoSlide();
    index = Math.max(index - 1, 0);
    updatecarousel();
    updateselectimg(index);
    //startAutoSlide();
}

function nextimg() {
    //stopAutoSlide();
    index = Math.min(index + 1, imgelements.length - 1);
    updatecarousel();
    updateselectimg(index);
    //startAutoSlide();
}

function updatecarousel() {
    container.style.transform = `translateX(-${index * imgwidth}px)`;
}

function updateselectimg(index) {
    selectimgelements.forEach((selectimgelement, i) => {
        if (i != index) {
            selectimgelement.classList.remove("btn_selected_img");
        }
        else {
            selectimgelement.classList.add("btn_selected_img");
        }
    })
}

window.addEventListener("load", () => {
    startAutoSlide();
    for (let i = 0; i < imgelements.length; i++) {
        //下方圖片顯示位置僅顯示第一個
        if (i == 0) {
            selectimgelements[i].classList.add("btn_selected_img");
        }

    }
})

selectimgelements.forEach(function (selectimgelement, selectindex) {
    selectimgelement.addEventListener("click", () => {
        stopAutoSlide();
        //下方點選點亮
        for (let i = 0; i < selectimgelements.length; i++) {
            if (i != selectindex) {
                selectimgelements[i].classList.remove("btn_selected_img");
            }
            else {
                selectimgelement.classList.add("btn_selected_img");
                index = selectindex;
            }
        }
        //圖片顯示
        updatecarousel();
        //startAutoSlide();
    })
});  