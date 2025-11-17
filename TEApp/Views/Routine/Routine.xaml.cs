using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace TEApp.Views.Routine
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string Period { get; set; }
        public bool IsCompleted { get; set; }
        public string Icon { get; set; }
    }

    public partial class Routine : ContentPage
    {
        private List<TaskItem> _tasks;
        private int _nextTaskId = 9;
        private const string TASKS_KEY = "routine_tasks";
        private const string NEXT_ID_KEY = "routine_next_id";

        public Routine()
        {
            InitializeComponent();
            LoadTasksFromStorage();
            LoadTasks();
            UpdateStatistics();
            UpdateDateLabel();
        }

        private void LoadTasksFromStorage()
        {
            try
            {
                // Carrega as tarefas salvas
                string tasksJson = Preferences.Get(TASKS_KEY, string.Empty);

                if (!string.IsNullOrEmpty(tasksJson))
                {
                    _tasks = JsonSerializer.Deserialize<List<TaskItem>>(tasksJson);
                    _nextTaskId = Preferences.Get(NEXT_ID_KEY, 9);
                }
                else
                {
                    // Se não há dados salvos, inicializa com tarefas padrão
                    InitializeTasks();
                    SaveTasksToStorage();
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, inicializa com tarefas padrão
                Console.WriteLine($"Erro ao carregar tarefas: {ex.Message}");
                InitializeTasks();
            }
        }

        private void SaveTasksToStorage()
        {
            try
            {
                string tasksJson = JsonSerializer.Serialize(_tasks);
                Preferences.Set(TASKS_KEY, tasksJson);
                Preferences.Set(NEXT_ID_KEY, _nextTaskId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar tarefas: {ex.Message}");
            }
        }

        private void InitializeTasks()
        {
            _tasks = new List<TaskItem>
            {
                // Manhã
                new TaskItem { Id = 1, Title = "Tomar café da manhã", Time = "07:00", Period = "Manhã", IsCompleted = false, Icon = "🍳" },
                new TaskItem { Id = 2, Title = "Tomar medicação", Time = "08:00", Period = "Manhã", IsCompleted = false, Icon = "💊" },
                new TaskItem { Id = 3, Title = "Exercícios de respiração", Time = "09:30", Period = "Manhã", IsCompleted = false, Icon = "🧘" },
                
                // Tarde
                new TaskItem { Id = 4, Title = "Almoço", Time = "12:30", Period = "Tarde", IsCompleted = false, Icon = "🍽️" },
                new TaskItem { Id = 5, Title = "Pausa sensorial", Time = "14:00", Period = "Tarde", IsCompleted = false, Icon = "🎧" },
                new TaskItem { Id = 6, Title = "Caminhada", Time = "15:00", Period = "Tarde", IsCompleted = false, Icon = "🚶" },
                
                // Noite
                new TaskItem { Id = 7, Title = "Jantar", Time = "19:00", Period = "Noite", IsCompleted = false, Icon = "🍲" },
                new TaskItem { Id = 8, Title = "Leitura relaxante", Time = "21:00", Period = "Noite", IsCompleted = false, Icon = "📖" }
            };
        }

        private void UpdateDateLabel()
        {
            var today = DateTime.Now;
            var monthNames = new[] { "", "janeiro", "fevereiro", "março", "abril", "maio", "junho",
                                    "julho", "agosto", "setembro", "outubro", "novembro", "dezembro" };
            DateLabel.Text = $"Hoje, {today.Day} de {monthNames[today.Month]}";
        }

        private void LoadTasks()
        {
            // Limpa os containers
            MorningTasksContainer.Children.Clear();
            AfternoonTasksContainer.Children.Clear();
            NightTasksContainer.Children.Clear();

            // Adiciona tarefas em cada período
            foreach (var task in _tasks.Where(t => t.Period == "Manhã").OrderBy(t => t.Time))
            {
                MorningTasksContainer.Children.Add(CreateTaskCard(task));
            }

            foreach (var task in _tasks.Where(t => t.Period == "Tarde").OrderBy(t => t.Time))
            {
                AfternoonTasksContainer.Children.Add(CreateTaskCard(task));
            }

            foreach (var task in _tasks.Where(t => t.Period == "Noite").OrderBy(t => t.Time))
            {
                NightTasksContainer.Children.Add(CreateTaskCard(task));
            }
        }

        private Frame CreateTaskCard(TaskItem task)
        {
            var frame = new Frame
            {
                BackgroundColor = task.IsCompleted ? Color.FromArgb("#F0F9FF") : Colors.White,
                CornerRadius = 15,
                Padding = 15,
                HasShadow = true,
                BorderColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Colors.Transparent
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(40, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(30, GridUnitType.Absolute) }
                },
                ColumnSpacing = 12
            };

            // Ícone da tarefa
            var iconFrame = new Frame
            {
                BackgroundColor = task.IsCompleted ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#F3F4F6"),
                CornerRadius = 10,
                Padding = 0,
                HasShadow = false,
                WidthRequest = 40,
                HeightRequest = 40
            };

            var iconLabel = new Label
            {
                Text = task.Icon,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            iconFrame.Content = iconLabel;
            Grid.SetColumn(iconFrame, 0);
            grid.Children.Add(iconFrame);

            // Conteúdo da tarefa
            var contentStack = new VerticalStackLayout
            {
                Spacing = 3,
                VerticalOptions = LayoutOptions.Center
            };

            var titleLabel = new Label
            {
                Text = task.Title,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = task.IsCompleted ? Color.FromArgb("#666") : Color.FromArgb("#3D3466"),
                TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : TextDecorations.None
            };

            var timeLabel = new Label
            {
                Text = $"⏰ {task.Time}",
                FontSize = 12,
                TextColor = Color.FromArgb("#999")
            };

            contentStack.Children.Add(titleLabel);
            contentStack.Children.Add(timeLabel);

            Grid.SetColumn(contentStack, 1);
            grid.Children.Add(contentStack);

            // Checkbox
            var checkFrame = new Frame
            {
                BackgroundColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Colors.White,
                CornerRadius = 15,
                Padding = 0,
                HasShadow = false,
                WidthRequest = 30,
                HeightRequest = 30,
                BorderColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Color.FromArgb("#DDD"),
                VerticalOptions = LayoutOptions.Center
            };

            var checkLabel = new Label
            {
                Text = task.IsCompleted ? "✓" : "",
                FontSize = 18,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            checkFrame.Content = checkLabel;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnTaskToggled(task, frame, titleLabel, checkFrame, checkLabel);
            checkFrame.GestureRecognizers.Add(tapGesture);

            Grid.SetColumn(checkFrame, 2);
            grid.Children.Add(checkFrame);

            frame.Content = grid;

            // Gesto de toque longo para editar/excluir
            var longPressGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            longPressGesture.Tapped += async (s, e) => await OnTaskLongPress(task);
            frame.GestureRecognizers.Add(longPressGesture);

            return frame;
        }

        private async void OnTaskToggled(TaskItem task, Frame frame, Label titleLabel, Frame checkFrame, Label checkLabel)
        {
            task.IsCompleted = !task.IsCompleted;

            // Animação
            await checkFrame.ScaleTo(1.2, 100);
            await checkFrame.ScaleTo(1, 100);

            // Atualiza visual
            frame.BackgroundColor = task.IsCompleted ? Color.FromArgb("#F0F9FF") : Colors.White;
            frame.BorderColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Colors.Transparent;
            checkFrame.BackgroundColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Colors.White;
            checkFrame.BorderColor = task.IsCompleted ? Color.FromArgb("#4CAF50") : Color.FromArgb("#DDD");
            checkLabel.Text = task.IsCompleted ? "✓" : "";
            titleLabel.TextColor = task.IsCompleted ? Color.FromArgb("#666") : Color.FromArgb("#3D3466");
            titleLabel.TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : TextDecorations.None;

            // Salva no storage
            SaveTasksToStorage();
            UpdateStatistics();

            // Mensagem motivacional
            if (task.IsCompleted)
            {
                var completedCount = _tasks.Count(t => t.IsCompleted);
                if (completedCount == _tasks.Count)
                {
                    await DisplayAlert("🎉 Parabéns!", "Você completou todas as tarefas do dia! Incrível!", "OK");
                }
            }
        }

        private async Task OnTaskLongPress(TaskItem task)
        {
            var action = await DisplayActionSheet(
                $"{task.Title}",
                "Cancelar",
                "Excluir",
                "Editar");

            if (action == "Excluir")
            {
                bool confirm = await DisplayAlert("Confirmar", $"Deseja excluir '{task.Title}'?", "Sim", "Não");
                if (confirm)
                {
                    _tasks.Remove(task);
                    SaveTasksToStorage();
                    LoadTasks();
                    UpdateStatistics();
                }
            }
            else if (action == "Editar")
            {
                await EditTask(task);
            }
        }

        private void UpdateStatistics()
        {
            var total = _tasks.Count;
            var completed = _tasks.Count(t => t.IsCompleted);
            var pending = total - completed;
            var progress = total > 0 ? (double)completed / total : 0;

            TotalTasksLabel.Text = total.ToString();
            CompletedTasksLabel.Text = completed.ToString();
            PendingTasksLabel.Text = pending.ToString();
            ProgressBar.Progress = progress;
            ProgressPercentLabel.Text = $"{(int)(progress * 100)}%";
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            // Título da tarefa
            string title = await DisplayPromptAsync(
                "Nova Tarefa",
                "Qual é a tarefa?",
                placeholder: "Ex: Estudar matemática");

            if (string.IsNullOrWhiteSpace(title))
                return;

            // Horário
            string time = await DisplayPromptAsync(
                "Horário",
                "Que horas? (HH:MM)",
                placeholder: "Ex: 14:30",
                keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(time))
                time = DateTime.Now.ToString("HH:mm");

            // Período do dia
            string period = await DisplayActionSheet(
                "Período do dia",
                "Cancelar",
                null,
                "Manhã",
                "Tarde",
                "Noite");

            if (period == "Cancelar" || string.IsNullOrEmpty(period))
                return;

            // Ícone
            string icon = await DisplayActionSheet(
                "Escolha um ícone",
                "Cancelar",
                null,
                "📚 Estudar", "💼 Trabalho", "🏃 Exercício", "🍎 Alimentação",
                "💊 Medicação", "🎯 Meta", "🧘 Relaxar", "🎨 Hobby",
                "📞 Ligação", "✉️ Email", "🛒 Compras", "🏠 Casa");

            string selectedIcon = icon switch
            {
                "📚 Estudar" => "📚",
                "💼 Trabalho" => "💼",
                "🏃 Exercício" => "🏃",
                "🍎 Alimentação" => "🍎",
                "💊 Medicação" => "💊",
                "🎯 Meta" => "🎯",
                "🧘 Relaxar" => "🧘",
                "🎨 Hobby" => "🎨",
                "📞 Ligação" => "📞",
                "✉️ Email" => "✉️",
                "🛒 Compras" => "🛒",
                "🏠 Casa" => "🏠",
                _ => "📝"
            };

            // Cria a nova tarefa
            var newTask = new TaskItem
            {
                Id = _nextTaskId++,
                Title = title,
                Time = time,
                Period = period,
                IsCompleted = false,
                Icon = selectedIcon
            };

            _tasks.Add(newTask);
            SaveTasksToStorage();
            LoadTasks();
            UpdateStatistics();

            await DisplayAlert("✅ Sucesso", $"Tarefa '{title}' adicionada!", "OK");
        }

        private async Task EditTask(TaskItem task)
        {
            // Editar título
            string newTitle = await DisplayPromptAsync(
                "Editar Tarefa",
                "Novo título:",
                initialValue: task.Title);

            if (string.IsNullOrWhiteSpace(newTitle))
                return;

            // Editar horário
            string newTime = await DisplayPromptAsync(
                "Editar Horário",
                "Novo horário (HH:MM):",
                initialValue: task.Time,
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrWhiteSpace(newTime))
                task.Time = newTime;

            task.Title = newTitle;
            SaveTasksToStorage();
            LoadTasks();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnCalendarClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Calendário", "Funcionalidade de calendário em breve!", "OK");
        }

        // Método opcional para resetar as tarefas para o padrão
        private void ResetTasks()
        {
            InitializeTasks();
            SaveTasksToStorage();
            LoadTasks();
            UpdateStatistics();
        }

        // Método opcional para limpar todos os dados
        private void ClearAllData()
        {
            Preferences.Remove(TASKS_KEY);
            Preferences.Remove(NEXT_ID_KEY);
            _tasks.Clear();
            LoadTasks();
            UpdateStatistics();
        }
    }
}