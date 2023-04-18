var downButton = document.querySelector(".down_button");

downButton.addEventListener("click", () => {
  fetch("https://j8b110.p.ssafy.io/api/v1/file/download/Runtopia.zip", {
    method: "GET",
  })
    .then(() => {
      console.log("파일 다운로드 성공");
    })
    .catch(() => {
      console.error("파일다운로드 실패");
    });
});
