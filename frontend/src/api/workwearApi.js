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
    const response = await fetch(`${API_URL}/api/workwear/request`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify({ 
            clothesSize: clothesSize !== '' && clothesSize !== null ? clothesSize.toString() : null, 
            shoesSize: shoesSize !== '' && shoesSize !== null ? shoesSize.toString() : null 
        })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || "Не удалось отправить запрос на изменение размеров");
    }

    return await response.json();
}