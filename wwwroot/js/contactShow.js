document.getElementById("show").addEventListener("click", function (event) {
  const colName = document.getElementById("col-name");
  const colNum = document.getElementById("col-num");

  event.target.disabled = true;
  colName.style.visibility = "visible";
  colNum.style.visibility = "visible";
});
