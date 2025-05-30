// JavaScript to handle dynamic behavior for Award forms

// Function to initialize the form
function initializeAwardForm() {
    const nomineeDropdown = document.getElementById('Nominee_Awardable_ID');
    const movieContextDropdown = document.getElementById('Movie_Context_ID');
    const movieContextContainer = document.getElementById('movie-context-container');

    if (!nomineeDropdown || !movieContextDropdown || !movieContextContainer) {
        console.error('One or more required elements not found.');
        return;
    }

    // Function to update Movie Context dropdown based on nominee selection
    function updateMovieContextDropdown() {
        // Get the selected option text to check if it's a Movie
        const selectedOption = nomineeDropdown.options[nomineeDropdown.selectedIndex];
        const isMovie = selectedOption && selectedOption.text && selectedOption.text.includes('(Movie)');

        if (isMovie) {
            // If nominee is a Movie, disable the Movie Context dropdown and set to null
            movieContextDropdown.disabled = true;
            movieContextDropdown.value = '';
            movieContextContainer.classList.add('disabled-field');

            // Add a note explaining why it's disabled
            let noteElement = document.getElementById('movie-context-note');
            if (!noteElement) {
                noteElement = document.createElement('small');
                noteElement.id = 'movie-context-note';
                noteElement.className = 'form-text text-muted';
                noteElement.innerHTML = 'Movie Context is not applicable when the nominee is a Movie';
                movieContextContainer.appendChild(noteElement);
            }
        } else {
            // If nominee is not a Movie, enable the dropdown
            movieContextDropdown.disabled = false;
            movieContextContainer.classList.remove('disabled-field');

            // Remove the note if it exists
            const noteElement = document.getElementById('movie-context-note');
            if (noteElement) {
                noteElement.remove();
            }
        }
    }

    // Set up the initial state
    updateMovieContextDropdown();

    // Add event listener for changes to the nominee dropdown
    nomineeDropdown.addEventListener('change', updateMovieContextDropdown);
}

// Call the initialization function when the document is ready
document.addEventListener('DOMContentLoaded', initializeAwardForm);
