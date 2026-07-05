import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getAllNews, deleteNews } from "../../api/newsApi";

export default function NewsManager() {
    const navigate = useNavigate();

    const [news, setNews] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadNews() {
            try {
                const data = await getAllNews();
                setNews(data);
            }
            finally {
                setLoading(false);
            }
        }

        loadNews();
    }, []);

    async function handleDelete(id) {
        if (!window.confirm("Удалить новость?"))
            return;

        try {
            await deleteNews(id);

            setNews(prev =>
                prev.filter(news => news.id !== id)
            );
        }
        catch (error) {
            alert(error.message);
        }
    }

    if (loading)
        return <p>Загрузка...</p>;

    return (
        <div>
            <h2>Новости</h2>

            <button
                onClick={() => navigate("/manager/news/create")}
            >
                Создать
            </button>

            <hr />

            {news.length === 0 && (
                <p>Новостей пока нет</p>
            )}

            <ul>
                {news.map(item => (
                    <li key={item.id}>
                        {item.title}

                        <button
                            onClick={() =>
                                navigate(`/manager/news/edit/${item.id}`)
                            }
                        >
                            Редактировать
                        </button>

                        <button
                            onClick={() => handleDelete(item.id)}
                        >
                            Удалить
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
}