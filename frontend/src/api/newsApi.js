import { API_URL } from "../config/api";

// 1. Заголовки для обычных запросов без файлов (например, удаление)
function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

// 2. Заголовки специально для FormData (создание и обновление новостей с картинками)
function getAuthHeadersForFormData() {
    const token = localStorage.getItem("token");
    return {
        // Content-Type НЕ пишем! Браузер сам выставит multipart/form-data вместе с boundary
        "Authorization": `Bearer ${token}`
    };
}

// Получить все новости (доступно гостям, токен не нужен)
export async function getAllNews() {
    const response = await fetch(`${API_URL}/api/news`);

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить новости");
    }

    return await response.json();
}

// Получить новость по ID (доступно гостям, токен не нужен)
export async function getNewsById(id) {
    const response = await fetch(`${API_URL}/api/news/${id}`);

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить новость");
    }

    return await response.json();
}

// Создать новость (передаем formData и заголовки авторизации)
export async function createNews(formData) {
    const response = await fetch(`${API_URL}/api/news`, {
        method: "POST",
        headers: getAuthHeadersForFormData(), // ❗ Заголовки для файлов
        body: formData
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось создать новость");
    }

    return response;
}

// Обновить новость (передаем formData и заголовки авторизации)
export async function updateNews(id, formData) {
    const response = await fetch(`${API_URL}/api/news/${id}`, {
        method: "PUT",
        headers: getAuthHeadersForFormData(), // ❗ Заголовки для файлов
        body: formData
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось обновить новость");
    }

    return response;
}

// Удалить новость (передаем обычные заголовки с токеном)
export async function deleteNews(id) {
    const response = await fetch(`${API_URL}/api/news/${id}`, {
        method: "DELETE",
        headers: getAuthHeaders() // ❗ Обычные заголовки авторизации
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось удалить новость");
    }

    return response;
}