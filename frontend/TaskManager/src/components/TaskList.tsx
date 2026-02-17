import { List } from "@mui/material";
import TaskItem from "./TaskItem";
import type { Task } from "../types/task";


interface TaskListProps {
  tasks: Task[];
}

const TaskList = ({ tasks }: TaskListProps) => {
  if (!tasks || tasks.length === 0) {
    return <p>Brak tasków</p>;
  }

  return (
    <List>
      {tasks.map((task) => (
        <TaskItem key={task.id} task={task} />
      ))}
    </List>
  );
};

export default TaskList;
