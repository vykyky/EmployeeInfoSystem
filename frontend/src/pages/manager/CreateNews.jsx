import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createNews } from "../../api/newsApi";

export default function CreateNews() {
    const navigate = useNavigate();

    const [title, setTitle] = useState("");
    const [body, setBody] = useState("");
    const [image, setImage] = useState(null);
    const [preview, setPreview] = useState(null);

    function handleImageChange(e) {
        const file = e.target.files[0];

        if (!file)
            return;

        setImage(file);
        setPreview(URL.createObjectURL(file));
    }

    async function handleSubmit() {
        try {
            const formData = new FormData();

            formData.append("title", title);
            formData.append("body", body);
            formData.append("authorId", 1);

            if (image)
                formData.append("image", image);

            await createNews(formData);

            navigate("/manager/news");
        }
        catch (error) {
            alert(error.message);
        }
    }

    return (
        <div>
            <h2>Создание новости</h2>

            {preview && (
                <img
                    src={preview}
                    alt="Превью"
                    width="200"
                />
            )}

            <input
                type="file"
                onChange={handleImageChange}
            />

            <br />

            <input
                value={title}
                onChange={e => setTitle(e.target.value)}
                placeholder="Заголовок"
            />

            <br />

            <textarea
                value={body}
                onChange={e => setBody(e.target.value)}
            />

            <br />

            <button onClick={handleSubmit}>
                Сохранить
            </button>
        </div>
    );
}