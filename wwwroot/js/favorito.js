document.addEventListener("DOMContentLoaded", function () {
    const btnFavorito = document.getElementById("btnFavorito");
    const iconoFavorito = document.getElementById("iconoFavorito");
    const favoritoInput = document.getElementById("favoritoInput");

    if (!btnFavorito || !iconoFavorito || !favoritoInput) {
        return;
    }
    // Leer valor real del hidden
    let esFavorito = favoritoInput.value === "true";

    btnFavorito.addEventListener("click", function () {

        esFavorito = !esFavorito;

        favoritoInput.value = esFavorito;

        if (esFavorito) {

            iconoFavorito.classList.remove("bi-star");
            iconoFavorito.classList.add("bi-star-fill");

        } else {

            iconoFavorito.classList.remove("bi-star-fill");
            iconoFavorito.classList.add("bi-star");

        }

    });
});