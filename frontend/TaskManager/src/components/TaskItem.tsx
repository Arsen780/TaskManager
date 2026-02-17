import { ListItem, ListItemText, Paper } from "@mui/material";
import type { Task } from "../types/task";

interface TaskItemProps {
  task: Task;
}

const TaskItem = ({ task }: TaskItemProps) => {
  return (
    <Paper sx={{ mb: 2, p: 2 }}>
      <ListItem>
        <ListItemText
          primary={task.title}
          secondary={`Status: ${task.status}`}
        />
      </ListItem>
    </Paper>
  );
};

export default TaskItem;
