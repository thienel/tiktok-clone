# TikTok Clone - Frontend

A React.js frontend application built with modern best practices and a well-organized code structure.

## 📋 Project Status

🚧 **Currently in Active Development**

### ✅ Completed

- Authentication system (login, register, password reset)
- State management with React Context
- Responsive UI components and styling
- Error handling and form validation
- Mobile-first design

### ⭭️ In Progress

- Video upload and playback system
- Video feed with infinite scroll
- Video interaction features (like, comment, share)

---

## 🚀 Features

### 🔐 Authentication

- User registration with email verification
- Login/logout with JWT
- Password reset
- Real-time username and birth date validation

### 🎨 UI & Design

- Mobile-first responsive layout
- Reusable components
- SCSS with CSS Modules
- Navigation sidebar
- Authentication modals

### ⚙️ State Management

- React Context for centralized state
- Global loading state coordination
- Centralized error handling
- API integration with validation

---

## 💠 Tech Stack

| Layer        | Stack                                                    |
| ------------ | -------------------------------------------------------- |
| Frontend     | React 19.1.0                                             |
| Routing      | React Router DOM 7.6.2                                   |
| HTTP Client  | Axios 1.9.0                                              |
| Styling      | SCSS + CSS Modules                                       |
| Build Tool   | React Scripts with `customize-cra` & `react-app-rewired` |
| Testing      | React Testing Library, Jest                              |
| Code Quality | Prettier, ESLint                                         |

---

## 🧠 Architecture Overview

### 🏗️ Clean Architecture

- **Component Organization**: Separation of concerns and intuitive structure
- **Hook-Based Logic**: Custom hooks for API, auth, debounce, theme, etc.
- **Context Providers**: Scalable global state without Redux
- **Unified Patterns**: Consistent file naming and structure

### ⭭️ Advanced State & Loading Coordination

**Problem**: Conflicting spinners and redundant loading states in complex apps
**Solution**: Centralized loading with `useLoading` and API-bound flags

```js
const { isAnyAuthLoading } = useLoading()
if (isAnyAuthLoading) return <AuthLoadingSpinner />
```

**Benefits:**

- ✅ No conflicting spinners
- ✅ Clear UX feedback
- ✅ Auto-cleanup after requests
- ✅ Specific loading indicators per context

---

## 📦 Installation

### Prerequisites

- Node.js v16+
- npm or yarn
- Backend API (for full functionality)

### Setup

```bash
git clone https://github.com/thienel/tiktok-clone-ui.git
cd tiktok-clone-ui/frontend
npm install
```

Create `.env` file:

```env
REACT_APP_API_BASE_URL=http://localhost:5062/api/
REACT_APP_ENV=development
```

Run the development server:

```bash
npm start
```

App runs at [http://localhost:3000](http://localhost:3000)

---

## 📁 Project Structure

```
src/
├── api/                 # API layer
├── components/          # Reusable UI components
├── context/             # Global state providers
├── hooks/               # Custom React hooks
├── utils/               # Helper and validation functions
└── styles/              # SCSS global styles
```

---

## ⛏️ Configuration

Uses `customize-cra` + `react-app-rewired` for:

- Path aliases (`~` = `src/`)
- SCSS with CSS Modules
- Custom font loading

Example:

```js
import Component from '~/components/Component'
```

---

## 🧚️‍♂️ Testing

- Jest and React Testing Library are preconfigured
- Supports unit and integration tests

---

## 🙏 Acknowledgments

- TikTok for UI inspiration
- React team for the powerful framework
- Open source community for tools & libraries

---

**Made with ❤️ by [thienel](https://github.com/thienel)**
_This project is intended solely for learning and experimental purposes._
