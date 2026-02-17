import { useEffect, useState } from "react";
import { Typography, CircularProgress } from "@mui/material";
import TaskList from "../components/TaskList";
import api from "../api/axiosConfig";
import type { Task } from "../types/task";

const TasksPage = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchTasks = async () => {
    try {
      setLoading(true);
      const response = await api.get("/tasks");
      setTasks(response.data.items);
    } catch (error) {
      console.error("Błąd pobierania tasków", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  return (
    <>
      <Typography variant="h4" sx={{ my: 3 }}>
        Moje taski
      </Typography>

      {loading ? <CircularProgress /> : <TaskList tasks={tasks} />}
    </>
  );
};

export default TasksPage;
