using System;
using System.Collections.Generic;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models.Null3rdLevelIdCase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd48Null3RdLevelIdTestCase : MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseMySql("TestCaseData/Null3rdLevelIdIssueCase");
        }

        protected override LoggerAdapter OnLoggerConfiguration(LoggerAdapter logger)
        {
            return logger.EnableAll();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {
            var exam = new ExamStorage
            {
                Id = Guid.NewGuid().ToString(),
                Level = 3,
                DurationMinutes = 10,
                MinimumAcceptancePoints = 40
            };

            var user = new UserStorage
            {
                Email = "goj_loj@yahoo.com",
                Id = Guid.NewGuid().ToString(),
                Level = 0,
                Name = "Mani",
                Surname = "Moayedi",
                HashedPassword = "hash",
                MobileNumber = "09386229511",
                NationalId = "9987362538",
                PhoneNumber = "09386229511",
                ProfilePictureId = Guid.NewGuid().ToString()
            };

            var questions = new List<ExamQuestionStorage>()
            {
                new ExamQuestionStorage
                {
                    Answers = "",
                    Id = Guid.NewGuid().ToString(),
                    Points = 2,
                    Question = "Hello?",
                    CorrectAnswer = 2,
                    ExamId = exam.Id
                },
                new ExamQuestionStorage
                {
                    Answers = "",
                    Id = Guid.NewGuid().ToString(),
                    Points = 2,
                    Question = "How?",
                    CorrectAnswer = 2,
                    ExamId = exam.Id
                }
            };

            var evaluation = new EvaluationSessionStorage
            {
                Duration = 1000,
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExamId = exam.Id,
                ExpiresAt = 100000,
                IsStarted = false,
                IsSubmitted = false,
            };

            Seed(engine, new ExamStorage[] { exam });

            Seed(engine, new UserStorage[] { user });

            Seed(engine, questions);

            Seed(engine, new EvaluationSessionStorage[] { evaluation });

            var request = new ReadByIdRequest<EvaluationSessionStorage, string>(evaluation.Id);

            var response = engine.PerformRequest(request, true);

            if (string.IsNullOrWhiteSpace(response.FromStorage[0].Exam?.Questions[1].ExamId))
            {
                throw new Exception("Question issue");
            }

            var userAnswer = new UserAnswerStorage
            {
                Choice = 2,
                Id = Guid.NewGuid().ToString(),
                EvaluationId = evaluation.Id,
                QuestionId = questions[0].Id,
                UserId = user.Id
            };

            var badRequest = new SaveRequest<UserAnswerStorage>(userAnswer);

            var badResponse = engine.PerformRequest(badRequest);

            // engine.PerformRequest(new DeleteById<UserAnswerStorage, string>(userAnswer.Id));
            
            response = engine.PerformRequest(request, true);

            if (string.IsNullOrWhiteSpace(response.FromStorage[0].Exam?.Questions[1].ExamId))
            {
                throw new Exception("Question issue");
            }
        }
    }
}