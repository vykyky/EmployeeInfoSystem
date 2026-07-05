import { API_URL } from "../config/api";

function getAuthHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

export async function getWorkwear() {
    const response = await fetch(`${API_URL}/api/workwear`, {
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось получить данные по спец. одежде");
    }

    return await response.json();
}

export async function sendWorkwearUpdateRequest(clothesSize, shoesSize) {
    const response = await fetch(`${API_URL}/api/workwear/request-change`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify({ clothesSize, shoesSize })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось отправить запрос на изменение размеров");
    }

    return await response.json();
}