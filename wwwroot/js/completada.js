
document.addEventListener("DOMContentLoaded", () => {

    const checkboxes = document.querySelectorAll(".tarea-check");
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    checkboxes.forEach(check => {

        check.addEventListener("change", async function () {

            const id = this.dataset.id;
            const completado = this.checked;

            try {

                const response = await fetch("/Home/CambiarEstado", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify({
                        id: id,
                        completado: completado
                    })
                });

                const data = await response.json();

                if (!data.success) {
                    alert(data.message);
                }

            } catch (error) {

                console.error("Error:", error);
            }
        });

    });

});