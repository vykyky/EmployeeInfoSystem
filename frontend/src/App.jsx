import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"

import ProtectedRoute from "./components/ProtectedRoute"
// PUBLIC
import Login from "./pages/public/Login"
import News from "./pages/public/News"
import NewsDetails from "./pages/public/NewsDetails"
import PublicLayout from "./layouts/PublicLayout"

// USER
import UserLayout from "./layouts/UserLayout"
import Profile from "./pages/user/Profile"
import Settings from "./pages/user/Settings"
import Notifications from "./pages/user/Notifications"
import Requests from "./pages/user/Requests"
import Workwear from "./pages/user/Workwear"

// MANAGER
import ManagerLayout from "./layouts/ManagerLayout"
import Tasks from "./pages/manager/Tasks"
import TaskDetails from "./pages/manager/TaskDetails"
import NewsManager from "./pages/manager/NewsManager"
import CreateNews from "./pages/manager/CreateNews"
import EditNews from "./pages/manager/EditNews"

// ADMIN
import AdminLayout from "./layouts/AdminLayout"
import WorkwearAdmin from "./pages/admin/WorkwearAdmin"
import PersonalInfoAdmin from "./pages/admin/PersonalInfoAdmin"
import TasksAdmin from "./pages/admin/TasksAdmin"
import TaskDetailsAdmin from "./pages/admin/TaskDetailsAdmin"
import RequestTypesAdmin from "./pages/admin/RequestTypesAdmin"
import UsersAdmin from "./pages/admin/UsersAdmin"

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        
        <Route element={<PublicLayout />}>
          <Route path="/" element={<Login />} />
          <Route path="/news" element={<News />} />
          <Route path="/news/:id" element={<NewsDetails />} />
        </Route>

        <Route path="/user" element={
          <ProtectedRoute>
            <UserLayout />
          </ProtectedRoute>
        }>
          
          <Route path="" element={<Profile />} /> 
          <Route path="profile" element={<Profile />} />
          <Route path="settings" element={<Settings />} />
          <Route path="notifications" element={<Notifications />} />
          <Route path="requests" element={<Requests />} />
          <Route path="workwear" element={<Workwear />} />
          <Route path="news" element={<News />} />
          <Route path="news/:id" element={<NewsDetails />} />
        </Route>


        <Route path="/manager" element={
          <ProtectedRoute role="manager">
            <ManagerLayout />
          </ProtectedRoute>
        }>
          <Route path="" element={<Tasks />} />
          <Route path="tasks" element={<Tasks />} />
          <Route path="tasks/:id" element={<TaskDetails />} />
          <Route path="news" element={<NewsManager />} />
          <Route path="news/create" element={<CreateNews />} />
          <Route path="news/edit/:id" element={<EditNews />} />
        </Route>


        <Route path="/admin" element={
          <ProtectedRoute role="admin">
            <AdminLayout />
          </ProtectedRoute>
        }>
          <Route path="" element={<WorkwearAdmin />} />
          <Route path="workwear" element={<WorkwearAdmin />} />
          <Route path="personal-info" element={<PersonalInfoAdmin />} />
          <Route path="tasks" element={<TasksAdmin />} />
          <Route path="tasks/:id" element={<TaskDetailsAdmin />} />
          <Route path="request-types" element={<RequestTypesAdmin />} />
          <Route path="users" element={<UsersAdmin />} />
        </Route>

        {/* Если ввели несуществующий адрес — кидаем на страницу логина */}
        <Route path="*" element={<Navigate to="/" replace />} />

      </Routes>
    </BrowserRouter>
  )
}