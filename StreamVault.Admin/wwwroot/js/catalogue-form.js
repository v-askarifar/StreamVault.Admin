(function () {
    const contentType = document.getElementById("contentType");

    function updateVisibleFields() {
        if (!contentType) {
            return;
        }

        const selected = contentType.value.toLowerCase();

        document.querySelectorAll(".type-field").forEach(function (field) {
            field.classList.add("d-none");
        });

        document.querySelectorAll(".type-" + selected).forEach(function (field) {
            field.classList.remove("d-none");
        });
    }

    function restrictNumericInputs() {
        document.querySelectorAll(".numeric-only").forEach(function (input) {
            input.addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, "");
            });

            input.addEventListener("paste", function (event) {
                const pasted = event.clipboardData.getData("text");

                if (!/^\d+$/.test(pasted)) {
                    event.preventDefault();
                }
            });

            input.addEventListener("keydown", function (event) {
                const allowedKeys = [
                    "Backspace",
                    "Delete",
                    "Tab",
                    "Escape",
                    "Enter",
                    "ArrowLeft",
                    "ArrowRight",
                    "Home",
                    "End"
                ];

                if (
                    allowedKeys.includes(event.key) ||
                    (event.ctrlKey && ["a", "c", "v", "x"].includes(event.key.toLowerCase()))
                ) {
                    return;
                }

                if (!/^\d$/.test(event.key)) {
                    event.preventDefault();
                }
            });
        });
    }

    if (contentType) {
        contentType.addEventListener("change", updateVisibleFields);
        updateVisibleFields();
    }

    restrictNumericInputs();
})();