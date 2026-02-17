export interface Task {
  id: string;
  name: string;
  description?: string;
  status: number;
  creationDate: string;
  deadline: string;
}
