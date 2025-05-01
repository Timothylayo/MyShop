let fieldCounter = 1; // Counter for dynamically added fields

document.getElementById('addFieldBtn').addEventListener('click', function () {
    fieldCounter++;

    // Create a new div to hold the field
    const newField = document.createElement('div');
    newField.classList.add('form-group');
    newField.setAttribute('id', 'fieldGroup' + fieldCounter);

    // Add the new input field
    newField.innerHTML = `
                <label for="field${fieldCounter}">Field ${fieldCounter}:</label>
                <input type="text" id="field${fieldCounter}" name="field${fieldCounter}" placeholder="Enter value">
                <button type="button" onclick="removeField(${fieldCounter})">Remove</button>
            `;

    // Append the new field to the container
    document.getElementById('newFieldsContainer').appendChild(newField);
});

function removeField(fieldId) {
    // Remove the field by its group id
    const fieldGroup = document.getElementById('fieldGroup' + fieldId);
    fieldGroup.remove();
}