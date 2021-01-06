using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace First
{
    delegate void MyEvent();
    class GameException : Exception
    {
        string message;
        public GameException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
    class Coach
    {
        public string name;
        public double level { get; set; }
        Random rand = new Random();
        public Coach(string name)
        {
            this.name = name;
            level = rand.Next(5, 16);
            level /= 10;
        }

    }
    
    class Referee
    {
        string name;
        Random rand = new Random();
        public int preference { get; set; }
        public Referee(string name)
        {
            this.name = name;
            preference = rand.Next(0, 3);
        }
        public void FoulHandler()
        {
            Console.Write("Игрок нарушил правила, судья показал желтую карточку.");
        }
        public void GoalHandler()
        {
            Console.WriteLine("Гоооол!!");
        }
    }
    class Team
    {
        public string team_name;
        public List<Footballer> list = new List<Footballer>();
        public double team_level { get; set; }
        public Coach coach;
        public int score { get; set; }
        public Team(string team_name, Coach coach)
        {
            this.team_name = team_name;
            this.coach = coach;
            score = 0;
        }
        public void CoachInfo()
        {
            Console.WriteLine($"Тренер команды {team_name} - {coach.name} с уровнем {coach.level}");
        }
        public void AddPlayer(Footballer ftb)
        {
            list.Add(ftb);
            team_level += ftb.level * coach.level;
        }
        public void RemovePlayer(int index, Team t)
        {
            Console.WriteLine($"С поля был удален игрок команды {t.team_name} - {t.list[index].name}");
            team_level -= list[index].level * coach.level;
            list.Remove(list[index]);            
        }
        public void ListAll()
        {
            int count = 0;
            Console.WriteLine($"Список всех игроков команды {team_name} по алфавиту:");
            Console.WriteLine("----------------------------------------------");
            var query = from item in list
                        orderby item.name
                        select item.name;
            foreach (var i in query)
                Console.WriteLine($"{++count}. {i}");
            Console.WriteLine();
        }
        public void List()
        {
            Console.WriteLine($"Информация об игроках команды {team_name}");
            Console.WriteLine("------------------------------------------");
            int counter = 1;
            var query = from item in list
                        orderby item.name
                        select item;
            Console.WriteLine($"{"Фамилия"} \t{"Возраст"}\t{"Уровень"}\t{"Желтые карточки"}");
            foreach (var i in query)
                Console.WriteLine($"{counter++}.{i.name}   \t{i.age}\t{i.level}\t{i.yellowCards}");
            Console.WriteLine();
        }
        public void ListOverThirty()
        {
            var query = from player in list
                        where player.age > 30
                        orderby player.level descending
                        select player;
            var count = (from q in query
                         select q).Count();
            try
            {
                if (count == 0)
                    throw new Exception($"В команде {team_name} отсутсвуют игроки старше 30 лет.");
                Console.WriteLine($"Список всех игроков команды {team_name} " +
                    $"старше 30 лет по убыванию уровня мастерства:");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine($"Фамилия\tВозраст\tУровень мастерства");
                foreach (var i in query)
                    Console.WriteLine($"{i.name}\t  {i.age}\t     { i.level}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
            }
        }
    }
    class Game
    {
        Team t1, t2;
        public event MyEvent Event;
        Referee referee;
        public Game(Team t1, Team t2, Referee referee)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.referee = referee;
        }
        public bool raining { get; set; }
        public void StartGame()
        {
            try
            {
                if (raining)
                    throw new GameException("Во время игры начался сильный дождь! Игра временно остановлена.");
                Event?.Invoke();
            }
            catch (GameException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void ResultGame()
        {
            double t1Level = t1.team_level;
            double t2Level = t2.team_level;
            if (referee.preference == 1)
            {
                Console.WriteLine($"Рефери поддерживает команду {t1.team_name}");
                t1Level += 40;
            }
            else if (referee.preference == 2)
            {
                Console.WriteLine($"Рефери поддерживает команду {t2.team_name}");
                t2Level += 40;
            }
            else
                Console.WriteLine($"Рефери не поддерживает никакую команду");
            double percent = (t1Level > t2Level) ? (double)t1Level / 10 : (double)t2Level / 10;
            TeamInfo(t1, t1Level);
            TeamInfo(t2, t2Level);
            if (t1Level - t2Level > percent)
                Console.WriteLine($"Победила команда {t1.team_name}");
            else if (t2Level - t1Level > percent)
                Console.WriteLine($"Победила команда {t2.team_name}");
            else
                Console.WriteLine("Ничья");
        }
        public void TeamInfo(Team t, double lev)
        {
            Console.WriteLine($"Уровень команды {t.team_name} - {lev:#.##}");
        }
    }
    class Footballer
    {
        public string name { get; set; }
        public int age { get; set; }
        public int level { get; set; }
        public int yellowCards;
        Random rand = new Random();
        public Footballer(string name, int age, int yellowCards = 0)
        {
            this.name = name;
            this.age = age;
            level = rand.Next(0, 101);
            this.yellowCards = yellowCards;
        }
    }
    class Score
    {
        Team t1, t2;
        public int footballer;
        public int team;
        public int number { get; set; }
        public Score(Team t1, Team t2)
        {
            this.t1 = t1;
            this.t2 = t2;
        }
        public void PlusScore()
        {
            Random r = new Random();
            number = r.Next(1, 3);
            if (number == 1)
                GoalScored(t1);
            else if (number == 2)
                GoalScored(t2);
        }
        public void GoalScored(Team t)
        {
            Random r = new Random();
            footballer = r.Next(0, t.list.Count);
            t.score++;
            Console.WriteLine($"Гол забил игрок команды {t.team_name} - {t.list[footballer].name}");
        }
        public void YellowCard()
        {
            Random r = new Random();
            Team t = (team == 1) ? t1 : t2;
            footballer = r.Next(0, t.list.Count);
            t.list[footballer].yellowCards++;
        }
        public void PrintYellowCard()
        {
            Console.Write(" Игрок команды ");
            Team t = (team == 1) ? t1 : t2;
            Console.WriteLine($"{t.team_name} - {t.list[footballer].name} ({t.list[footballer].yellowCards})");
            if (t.list[footballer].yellowCards == 2)
                t.RemovePlayer(footballer, t);
            
        }
        public void PrintScore()
        {
            Console.WriteLine($"Счет: {t1.score}:{t2.score}");
        }
        public void Winner()
        {
            if (t1.score > t2.score)
                Console.WriteLine($"Выиграла команда {t1.team_name}");
            else if (t1.score < t2.score)
                Console.WriteLine($"Выиграла команда {t2.team_name}");
            else
                Console.WriteLine("Ничья!");
        }
    }
    class Fans
    {
        Team t;
        int quantity;
        public Fans(Team t, int quantity)
        {
            this.quantity = quantity;
            this.t = t;
        }
        public void FansInfo()
        {
            Console.WriteLine($"Поддержать команду {t.team_name} собралось {quantity} фанатов!");
        }
    }
    class Program
    {
        delegate void Del();
        static void Main(string[] args)
        {
            Team team1 = new Team("Мурзилки", new Coach("Петров"));
            Team team2 = new Team("Пупсики", new Coach("Иванов"));

            team1.AddPlayer(new Footballer("Месси", 30));
            team1.AddPlayer(new Footballer("Шевченко", 20));
            team2.AddPlayer(new Footballer("Роналду", 41));
            team2.AddPlayer(new Footballer("Суарес", 39));
            team2.AddPlayer(new Footballer("Неймар", 29));
            team1.AddPlayer(new Footballer("Санчес", 20));
            team1.AddPlayer(new Footballer("Бонуччи", 49));
            Referee referee = new Referee("Александров");

            Game newGame = new Game(team1, team2, referee);
            Del del = team1.CoachInfo;
            del += team2.CoachInfo;
            del();

            Console.WriteLine();

            newGame.ResultGame();

            del = team1.ListAll;
            del += team2.ListAll;
            del += team1.ListOverThirty;
            del += team2.ListOverThirty;
            del();

            Score score = new Score(team1, team2);
            Console.WriteLine("Игра началась");
            Fans team1Fans = new Fans(team1, 1000);
            Fans team2Fans = new Fans(team2, 500);
            team1Fans.FansInfo();
            team2Fans.FansInfo();
            Console.WriteLine();
            Console.WriteLine($"{team1.team_name} - {team2.team_name}");
            score.PrintScore();
            Random r = new Random();
            for (int i = 0; i < 6; i++)
            {
                newGame.Event += referee.GoalHandler;
                newGame.Event += score.PlusScore;
                score.number= r.Next(1, 101);
                score.team = r.Next(1, 3);
                if (score.number < 20)
                {
                    newGame.Event += score.YellowCard;
                    newGame.Event += referee.FoulHandler;
                    newGame.Event += score.PrintYellowCard;
                }
                newGame.Event += score.PrintScore;    
            }
            newGame.StartGame();
            score.Winner();
            newGame.raining = false;
            team1.List();
            team2.List();
        }
    }
}
