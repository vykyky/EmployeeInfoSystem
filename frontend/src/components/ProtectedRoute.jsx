import { Navigate } from "react-router-dom";
import { isAuthenticated, getRole } from "../api/authApi";

// Использование:
// <ProtectedRoute role="admin">   — только для админа
// <ProtectedRoute role="manager"> — только для менеджера
// <ProtectedRoute>                — любой авторизованный

export default function ProtectedRoute({ children, role }) {
    if (!isAuthenticated()) {
        return <Navigate to="/" replace />;
    }

    if (role && getRole() !== role) {
        return <Navigate to="/" replace />;
    }

    return children;
}