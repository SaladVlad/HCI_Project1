# Content Management System (CMS)

## Overview
This project is a WPF application representing a simplified (desktop, local) content management system on a chosen topic. It provides functionalities such as tabular display of content, viewing, adding, deleting, and modifying content.

## User Roles
- **Admin**: Can add, modify, and delete content objects within the system.
- **Visitor**: Can only view information about added content.

## Features
### Login System
- Predefined usernames and passwords are provided for both user types.
- Users are classified as either Admin or Visitor.
- Interface and functionalities are adjusted based on the logged-in user.
- User data is serialized into a separate file.

### Content Management
- The system handles objects of a single class.
- Each object contains fields such as numeric, textual, image display, reference to an *.rtf file, and addition date.
- Objects are stored in a common *.xml file.

### User Interface
- After login, users are presented with a window/page displaying content objects in a tabular format.
- Buttons for adding, deleting, and exiting the program are provided.
- The tabular view includes checkboxes for selecting objects to delete, hyperlinks, images, and addition dates.

### Adding and Editing Objects
- Clicking the add button opens a form for selecting and entering values for all fields of a new object.
- A preview of the selected image is displayed.
- One text input field is implemented as a RichTextBox acting as a text editor.
- Font formatting options such as Bold, Italic, Underline, font color, and size are provided.
- Word count is displayed in the RichTextBox status bar.
- Upon successful addition, the addition date is automatically set.

### Deleting Objects
- Clicking the delete button removes selected objects from the tabular view.

### Hyperlink Interaction
- Clicking on a hyperlink opens different windows/pages based on the user type:
  - For Admin: Opens a window/page for editing, identical to the add window/page but with pre-filled fields.
  - For Visitor: Opens a window/page for detailed content display.

### Content Display Page
- Presents a clear layout of all fields of a specific content object.
- Fields are read-only.
- No visible controls except for informational display.
- Designed specifically for content display.

## Notes
- The add and edit windows/pages are identical, allowing for reuse.
- Functionalities available to each user type are carefully managed to ensure a user-appropriate interface.
- Aesthetic elements and theme adaptation are crucial, including color palettes, images, fonts, window shape, icons, and animations.
- Validation is implemented for all input fields and selections, with meaningful and tailored error messages displayed below each field.
- Confirmation is requested before deleting an object.
- Feedback is provided for every successful action, either through MessageBox or Toast notifications.
- Mouse cursor changes and tooltips with relevant content are used for user guidance.
- DataBinding is implemented within the tabular view.
- Content within RichTextBox is only displayed within it and saved in *.rtf format.
- C# naming conventions and clean code practices are followed consistently throughout the project.
- English is used consistently for both code and user interface.
- The project is presented with at least three pre-filled entities relevant to the theme, along with theme-adapted images.
- A thorough understanding of every line of code and its purpose is required for the defense. Plagiarism or lack of understanding will result in point deductions.

## Inspiration
- WordPress Dashboard
- Joomla Admin Panel
