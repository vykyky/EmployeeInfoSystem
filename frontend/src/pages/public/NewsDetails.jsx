import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getNewsById } from "../../api/newsApi";
import { API_URL } from "../../config/api";

import "./NewsDetails.css";

export default function NewsDetails() {
  const navigate = useNavigate();
  const { id } = useParams();

  const [item, setItem] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const isAuthenticated = !!localStorage.getItem("token");
  const newsListPath = isAuthenticated ? "/user/news" : "/news";

  useEffect(() => {
    async function loadNews() {
      setLoading(true);
      setError("");
      try {
        const data = await getNewsById(id);
        setItem(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadNews();
  }, [id]);

  if (loading) return <p>Загрузка...</p>;
  if (error) return <p>{error}</p>;
  if (!item) return null;

  return (
    <div className="news-details-page">
      <div className="news-details-main">

        <button
          className="news-back-btn"
          onClick={() => navigate(newsListPath)}
        >
          ← Назад к списку
        </button>

        <h1 className="news-details-title">{item.title}</h1>

        <small className="news-date">
          {new Date(item.createdAt).toLocaleDateString("ru-RU")}
        </small>

        {item.imagePath && (
          <div className="news-details-image">
            <img src={`${API_URL}/${item.imagePath}`} />
          </div>
        )}

        <p className="news-details-body">{item.body}</p>

      </div>
    </div>
  );
}