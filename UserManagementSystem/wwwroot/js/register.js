// wwwroot/js/register.js

document.addEventListener('DOMContentLoaded', function () {

    // Password toggle function
    window.togglePassword = function (id) {
        const input = document.getElementById(id);
        const icon = event.target.closest('button').querySelector('i');
        if (input.type === "password") {
            input.type = "text";
            icon.classList.remove('bi-eye');
            icon.classList.add('bi-eye-slash');
        } else {
            input.type = "password";
            icon.classList.remove('bi-eye-slash');
            icon.classList.add('bi-eye');
        }
    };

    // Populate static hobbies checkboxes
    const hobbies = ["Reading", "Sports", "Music", "Travel", "Cooking", "Gaming"];
    const container = document.getElementById('hobbiesContainer');
    if (container) {
        hobbies.forEach((hobby, index) => {
            const div = document.createElement('div');
            div.className = "form-check form-check-inline";

            const input = document.createElement('input');
            input.type = "checkbox";
            input.className = "form-check-input";
            input.name = "Hobbies";
            input.id = "hobby" + index;
            input.value = hobby;

            const label = document.createElement('label');
            label.className = "form-check-label";
            label.htmlFor = input.id;
            label.textContent = hobby;

            div.appendChild(input);
            div.appendChild(label);
            container.appendChild(div);
        });
    }

    // State -> City dropdown
    const stateDropdown = document.getElementById('stateDropdown');
    const cityDropdown = document.getElementById('cityDropdown');
    if (stateDropdown) {
        stateDropdown.addEventListener('change', function () {
            const stateId = this.value;
            if (!cityDropdown) return;

            if (stateId) {
                fetch(`/Account/GetCities?stateId=${stateId}`)
                    .then(response => response.json())
                    .then(cities => {
                        cityDropdown.innerHTML = '<option value="">City *</option>';
                        cities.forEach(city => {
                            const option = document.createElement('option');
                            option.value = city.id;
                            option.text = city.name;
                            cityDropdown.add(option);
                        });
                        cityDropdown.disabled = false;
                    });
            } else {
                cityDropdown.innerHTML = '<option value="">City *</option>';
                cityDropdown.disabled = true;
            }
        });
    }

    // Username validation
    const userNameInput = document.getElementById('UserName');
    if (userNameInput) {
        userNameInput.addEventListener('blur', function () {
            const username = this.value;
            if (!username) return;

            fetch(`/Account/CheckUsername?username=${username}`)
                .then(response => response.json())
                .then(isAvailable => {
                    const msg = userNameInput.nextElementSibling;
                    if (!isAvailable) {
                        userNameInput.classList.add('is-invalid');
                        msg.textContent = 'Username already exists';
                    } else {
                        userNameInput.classList.remove('is-invalid');
                        msg.textContent = '';
                    }
                });
        });
    }

});
