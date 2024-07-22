document.getElementById("images").addEventListener("change", function (event) {
  const files = event.target.files;
  const errorMessage = document.getElementById("warnMessage");
  errorMessage.classList.add("alert-info");
  errorMessage.classList.remove("alert-danger");

  //errorMessage.textContent = "";
  //errorMessage.style.visibility = "hidden";
  let validFiles = true;

  Array.from(files).forEach((file) => {
    const img = new Image();
    img.onload = function () {
      if (img.width < 550 || img.width <= img.height) {
        validFiles = false;
        //errorMessage.textContent =
        //  "Todas as imagens devem ter pelo menos 550 pixels de largura.";
        errorMessage.classList.add("alert-danger");

        errorMessage.classList.remove("alert-info");
        //errorMessage.style.visibility = "visible";
        event.target.value = "";
      }
    };
    img.src = URL.createObjectURL(file);
  });
});

document.getElementById("image").addEventListener("change", function (event) {
  const file = event.target.files[0];
  console.log(event.target.files.length);
  const errorMessage = document.getElementById("warnMessage");
  //errorMessage.textContent = ""; // Clear previous error message
  errorMessage.classList.add("alert-info");
  errorMessage.classList.remove("alert-danger");

  if (file) {
    const img = new Image();
    img.onload = function () {
      if (img.width < 550 || img.width <= img.height) {
        validFiles = false;
        //errorMessage.textContent =
        //  "Todas as imagens devem ter pelo menos 550 pixels de largura.";
        errorMessage.classList.add("alert-danger");

        errorMessage.classList.remove("alert-info");
        //errorMessage.style.visibility = "visible";
        event.target.value = "";
      }
    };
    img.src = URL.createObjectURL(file);
  }
});

document
  .getElementsByClassName("size-control")
  .addEventListener("change", function (event) {
    const errorMessage = document.getElementById("warnMessage");
    errorMessage.classList.add("alert-info");
    errorMessage.classList.remove("alert-danger");

    if (event.target.files.length > 1) {
    } else {
      const file = event.target.files[0];
      console.log(event.target.files.length);
      const errorMessage = document.getElementById("warnMessage");
      //errorMessage.textContent = ""; // Clear previous error message
      errorMessage.classList.add("alert-info");
      errorMessage.classList.remove("alert-danger");

      if (file) {
        const img = new Image();
        img.onload = function () {
          if (img.width < 550 || img.width <= img.height) {
            validFiles = false;
            //errorMessage.textContent =
            //  "Todas as imagens devem ter pelo menos 550 pixels de largura.";
            errorMessage.classList.add("alert-danger");

            errorMessage.classList.remove("alert-info");
            //errorMessage.style.visibility = "visible";
            event.target.value = "";
          }
        };
        img.src = URL.createObjectURL(file);
      }
    }
  });
