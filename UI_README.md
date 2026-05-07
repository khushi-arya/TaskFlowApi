# Task Manager UI Setup Guide

## Overview
A modern, responsive web-based UI for your Task Manager application built with HTML, CSS, and vanilla JavaScript.

## What Was Created

### 1. **UI Interface** 
- **Location**: `TaskManager.API/wwwroot/index.html`
- A single-page application with a clean, modern design
- Features:
  - ✅ Create new tasks with title, description, priority, and due date
  - ✅ View all tasks in a card-based list
  - ✅ Mark tasks as completed
  - ✅ Delete tasks
  - ✅ Edit tasks (placeholder for future implementation)
  - ✅ Real-time status updates
  - ✅ Responsive design (works on desktop, tablet, mobile)

### 2. **API Configuration**
- **File Modified**: `TaskManager.API/Program.cs`
- Added static file serving middleware
- Root path (`/`) now serves the UI interface
- CORS is already configured to allow requests

## How to Run

### Prerequisites
- .NET 8 SDK installed
- Visual Studio Code or Visual Studio

### Steps

1. **Build the Project**
   ```bash
   dotnet build
   ```

2. **Run the API Server**
   ```bash
   dotnet run --project TaskManager.API
   ```
   
   The API will start on `http://localhost:5000` by default.

3. **Access the UI**
   - Open your browser and navigate to: `http://localhost:5000/`
   - You should see the Task Manager UI

4. **Use the Application**
   - **Create Task**: Fill in the form with title (required), description, priority, and due date, then click "Add Task"
   - **View Tasks**: All tasks appear in the "Your Tasks" section
   - **Complete Task**: Click the "✓ Complete" button on any task
   - **Delete Task**: Click the "Delete" button to remove a task
   - **Edit Task**: Edit functionality will be added in future updates

## UI Features

### Modern Design
- Purple gradient background
- Clean white card-based layout
- Smooth animations and transitions
- Color-coded priority badges (Low, Medium, High)
- Status badges (Pending, Completed)

### Responsive Layout
- Mobile-friendly design
- Adapts to all screen sizes
- Touch-friendly buttons

### Real-time Feedback
- Success/error messages
- Loading indicators
- Form validation

## API Integration

The UI communicates with your existing API endpoints:
- `GET /api/tasks` - Fetch all tasks
- `POST /api/tasks` - Create a new task
- `GET /api/tasks/{id}` - Get task details
- `PUT /api/tasks/{id}` - Update task (ready for future implementation)
- `DELETE /api/tasks/{id}` - Delete a task
- `POST /api/tasks/{id}/complete` - Mark task as completed

## File Structure

```
TaskManager.API/
├── wwwroot/
│   └── index.html          # UI Interface
├── Program.cs              # Updated with static file serving
├── Controllers/
├── Properties/
└── TaskManager.API.csproj
```

## Browser Compatibility
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Notes
- The API expects to run on `http://localhost:5000/`
- If you're running on a different port, update the `API_BASE` variable in the JavaScript code (line ~375 in index.html)
- The database (SQLite) is created automatically on first run
- All data is persisted in `taskmanager.db`

## Future Enhancements
- ✏️ Full edit task functionality
- 🔍 Task search and filtering
- 📊 Task statistics dashboard
- 👥 User authentication
- 🔔 Notifications and reminders
- 📱 Progressive Web App (PWA) support
