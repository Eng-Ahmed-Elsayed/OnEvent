# OnEvent

## Milestones: 

### Milestone 1: Project Setup and Initial Planning

* Task: Set up the ASP.NET Core MVC project structure.
* Task: Integrate Bootstrap 5 into the project for styling.
* Task: Initialize a version control system (e.g., Git) repository for the project.
* Task: Create an initial backlog of user stories based on the project requirements.
* Task: Hold a sprint planning meeting to prioritize and estimate user stories for the first sprint.

### Milestone 2: Backend Development

* Task: Develop models for events, users, invitations, guest lists, etc.
* Task: Implement CRUD operations for managing events and user profiles.
* Task: Set up authentication and authorization using ASP.NET Core Identity.
* Task: Write unit tests for backend functionalities, focusing on models, controllers, and services.

### Milestone 3: Frontend Development

* Task: Design and implement UI wireframes for event creation, invitation management, and guest list management.
* Task: Develop frontend components using Razor Views and Bootstrap 5 classes.
* Task: Implement client-side validation for forms using JavaScript or jQuery.
* Task: Write integration tests to ensure proper interaction between frontend and backend components.

### Milestone 4: MVP Release

* Task: Complete the implementation of core features such as event creation, invitation sending, and guest list management.
* Task: Conduct user acceptance testing (UAT) with a small group of beta users.
* Task: Address any feedback and bugs identified during UAT.
* Task: Deploy the MVP to a staging environment for final testing.

### Milestone 5: Iterative Development and Refinement

* Task: Plan and execute subsequent sprints based on user feedback and project priorities.
* Task: Continuously refactor code and improve test coverage to maintain code quality.
* Task: Implement additional features and enhancements based on user stories and backlog prioritization.
* Task: Conduct regular sprint reviews to demo completed work and gather feedback.

## Backlog of user stories

1. User Authentication and Authorization:

    * As a user, I want to be able to register for an account so that I can access the event planning system.
    * As a user, I want to be able to log in to my account using my email and password.
    * As a user, I want to be able to reset my password if I forget it.
    * As an admin, I want to be able to manage user roles and permissions.

2. Event Creation and Management:

    * As a user, I want to be able to create a new event by providing event details such as title
    , locationType, description, date, time, and location. => Done
    * As a user, I want to be able to review the details of an existing event. => Done
    * As a user, I want to be able to edit the details of an existing event. => Done
    * As a user, I want to be able to delete an event that I created. => Done
    * As a user, I want to be able to view a list of all events that I have created or RSVP'd to. => Done
    * As a user, I want to be able to search for events by title, date, or location. => Done

3. Invitation Management:

    * As a user, I want to be able to send invitations to guests by email, including event details and RSVP options.
    * As a user, I want to be able to view a list of invited guests for each event.
    * As a user, I want to be able to track RSVP responses from invited guests.
    * As a user, I want to be able to send reminder emails to guests who have not yet responded to the invitation.

4. Guest List Management:

    * As a user, I want to be able to manually add guests to the guest list for an event.
    * As a user, I want to be able to import guest lists from external sources (e.g., CSV file).
    * As a user, I want to be able to view and manage the guest list for each event, including RSVP status and meal * preferences.
    * As a user, I want to be able to send updates or notifications to guests regarding event details or changes.

5. Reporting and Analytics:

    * As a user, I want to be able to generate reports on event attendance, RSVP statistics, and guest demographics.
    * As a user, I want to be able to export event data to a CSV or Excel file for further analysis.
    * As a user, I want to be able to view graphical representations of event data (e.g., charts, graphs).

6. Notifications and Reminders:

    * As a user, I want to receive email notifications for upcoming events, RSVP deadlines, or changes to event details.
    * As a user, I want to be able to configure notification settings (e.g., frequency, preferred communication channels).

7. Integration with Calendar Services:

    * As a user, I want to be able to synchronize event details with my calendar (e.g., Google Calendar, Outlook).
    * As a user, I want to receive notifications and reminders for events directly in my calendar application.
