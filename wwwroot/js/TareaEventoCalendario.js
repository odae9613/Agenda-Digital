function abrirModalEvento(id) {

    $.ajax({
        url: '/Evento/Detalle',
        type: 'GET',
        data: { id: id },

        success: function (html) {

            $('#modalBody').html(html);

            var modal = new bootstrap.Modal(
                document.getElementById('modalCalendario')
            );

            modal.show();
        },

        error: function () {
            alert('Error al cargar el evento');
        }
    });
}

function abrirModalTarea(id) {

    $.ajax({
        url: '/Tarea/Detalle',
        type: 'GET',
        data: { id: id },

        success: function (html) {

            $('#modalBody').html(html);

            var modal = new bootstrap.Modal(
                document.getElementById('modalCalendario')
            );

            modal.show();
        },

        error: function () {
            alert('Error al cargar tarea');
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {

    const colores = {
        Evento: {
            bg: "#8FB9B3",
            border: "#6D9B95"
        },
        Tarea: {
            bg: "#D6B6D5",
            border: "#B58DB6"
        }
    };

    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {

        initialView: 'dayGridMonth',

        locale: 'es-ES',

        //events: '/Calendario/GetTareasEventos',
        events: function (fetchInfo, successCallback, failureCallback) {

            fetch('/Calendario/GetTareasEventos')
                .then(r => r.json())
                .then(data => {

                    console.log("EVENTOS:", data);

                    successCallback(data);
                })
                .catch(err => {

                    console.error(err);
                    failureCallback(err);
                });
        },
        ////////////////////////////

        editable: true,

        selectable: true,

        eventClick: function (info) {

            info.jsEvent.preventDefault();

            const tipo = info.event.extendedProps?.tipo;
            const id = info.event.id;

            if (!tipo) {
                console.error("Evento sin tipo:", info.event);
                return;
            }

            if (tipo === "Evento") {
                abrirModalEvento(info.event.id);
            } else if (tipo === "Tarea") {
                abrirModalTarea(info.event.id);
            }
        }
    });

    calendar.render();
});
