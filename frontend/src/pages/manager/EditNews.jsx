import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getNewsById, updateNews } from "../../api/newsApi";
import { API_URL } from "../../config/api";

export default function EditNews() {
    const { id } = useParams();
    const navigate = useNavigate();

    const [title, setTitle] = useState("");
    const [body, setBody] = useState("");
    const [image, setImage] = useState(null);
    const [preview, setPreview] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadNews() {
            try {
                const data = await getNewsById(id);

                setTitle(data.title);
                setBody(data.body);

                if (data.imagePath) {
                    setPreview(
                        `${API_URL}/${data.imagePath}`
                    );
                }
            }
            finally {
                setLoading(false);
            }
        }

        loadNews();
    }, [id]);

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

            formData.append("id", id);
            formData.append("title", title);
            formData.append("body", body);

            if (image)
                formData.append("image", image);

            await updateNews(id, formData);

            navigate("/manager/news");
        }
        catch (error) {
            alert(error.message);
        }
    }

    if (loading)
        return <p>Загрузка...</p>;

    return (
        <div>
            <h2>Редактирование новости</h2>

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