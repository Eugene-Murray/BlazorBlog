using NSubstitute;

namespace UnitTestExamples
{
    // Example classes to test
    public interface IEmailService
    {
        bool SendEmail(string to, string subject, string body);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }

    public interface IUserRepository
    {
        User? GetUserById(int id);
        IEnumerable<User> GetAllUsers();
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public User? GetUser(int id)
        {
            return _userRepository.GetUserById(id);
        }

        public bool NotifyUser(int userId, string message)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null || !user.IsActive)
                return false;

            return _emailService.SendEmail(user.Email, "Notification", message);
        }

        public int GetActiveUserCount()
        {
            var users = _userRepository.GetAllUsers();
            return users.Count(u => u.IsActive);
        }
    }

    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
        public int Multiply(int a, int b) => a * b;
        public int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            return a / b;
        }
    }

    // Unit Tests
    public class UnitTestMockingExamples
    {
        #region Basic AAA Pattern Examples

        [Fact]
        public void Add_TwoPositiveNumbers_ReturnsSum()
        {
            // Arrange
            var calculator = new Calculator();
            int number1 = 5;
            int number2 = 3;

            // Act
            int result = calculator.Add(number1, number2);

            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void Subtract_TwoNumbers_ReturnsDifference()
        {
            // Arrange
            var calculator = new Calculator();
            int number1 = 10;
            int number2 = 4;

            // Act
            int result = calculator.Subtract(number1, number2);

            // Assert
            Assert.Equal(6, result);
        }

        [Fact]
        public void Divide_ByZero_ThrowsException()
        {
            // Arrange
            var calculator = new Calculator();
            int number1 = 10;
            int number2 = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => calculator.Divide(number1, number2));
        }

        #endregion

        #region NSubstitute Examples - Basic Mocking

        [Fact]
        public void GetUser_ValidId_ReturnsUser()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();
            var expectedUser = new User { Id = 1, Name = "John Doe", Email = "john@example.com" };

            mockRepository.GetUserById(1).Returns(expectedUser);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.GetUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public void GetUser_InvalidId_ReturnsNull()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            mockRepository.GetUserById(999).Returns((User?)null);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.GetUser(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region NSubstitute Examples - Verify Method Calls

        [Fact]
        public void NotifyUser_ActiveUser_SendsEmailAndReturnsTrue()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            var user = new User 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john@example.com", 
                IsActive = true 
            };

            mockRepository.GetUserById(1).Returns(user);
            mockEmailService.SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.NotifyUser(1, "Test message");

            // Assert
            Assert.True(result);
            mockEmailService.Received(1).SendEmail("john@example.com", "Notification", "Test message");
        }

        [Fact]
        public void NotifyUser_InactiveUser_DoesNotSendEmail()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            var user = new User 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john@example.com", 
                IsActive = false 
            };

            mockRepository.GetUserById(1).Returns(user);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.NotifyUser(1, "Test message");

            // Assert
            Assert.False(result);
            mockEmailService.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void NotifyUser_UserNotFound_DoesNotSendEmail()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            mockRepository.GetUserById(999).Returns((User?)null);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.NotifyUser(999, "Test message");

            // Assert
            Assert.False(result);
            mockEmailService.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        #endregion

        #region NSubstitute Examples - Working with Collections

        [Fact]
        public void GetActiveUserCount_WithActiveUsers_ReturnsCorrectCount()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            var users = new List<User>
            {
                new User { Id = 1, Name = "User 1", IsActive = true },
                new User { Id = 2, Name = "User 2", IsActive = false },
                new User { Id = 3, Name = "User 3", IsActive = true },
                new User { Id = 4, Name = "User 4", IsActive = true }
            };

            mockRepository.GetAllUsers().Returns(users);

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.GetActiveUserCount();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void GetActiveUserCount_WithNoUsers_ReturnsZero()
        {
            // Arrange
            var mockRepository = Substitute.For<IUserRepository>();
            var mockEmailService = Substitute.For<IEmailService>();

            mockRepository.GetAllUsers().Returns(new List<User>());

            var userService = new UserService(mockRepository, mockEmailService);

            // Act
            var result = userService.GetActiveUserCount();

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region NSubstitute Examples - Argument Matching

        [Fact]
        public void SendEmail_WithSpecificArguments_VerifiesExactValues()
        {
            // Arrange
            var mockEmailService = Substitute.For<IEmailService>();
            mockEmailService.SendEmail("test@example.com", "Test Subject", "Test Body").Returns(true);

            // Act
            var result = mockEmailService.SendEmail("test@example.com", "Test Subject", "Test Body");

            // Assert
            Assert.True(result);
            mockEmailService.Received().SendEmail("test@example.com", "Test Subject", "Test Body");
        }

        [Fact]
        public void SendEmail_WithAnyArguments_VerifiesCallMade()
        {
            // Arrange
            var mockEmailService = Substitute.For<IEmailService>();
            mockEmailService.SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            // Act
            mockEmailService.SendEmail("any@example.com", "Any Subject", "Any Body");

            // Assert
            mockEmailService.Received().SendEmail(
                Arg.Any<string>(), 
                Arg.Any<string>(), 
                Arg.Any<string>()
            );
        }

        [Fact]
        public void SendEmail_WithConditionalArguments_VerifiesMatchingConditions()
        {
            // Arrange
            var mockEmailService = Substitute.For<IEmailService>();
            mockEmailService.SendEmail(
                Arg.Is<string>(email => email.Contains("@")), 
                Arg.Any<string>(), 
                Arg.Any<string>()
            ).Returns(true);

            // Act
            mockEmailService.SendEmail("valid@example.com", "Subject", "Body");

            // Assert
            mockEmailService.Received().SendEmail(
                Arg.Is<string>(email => email.Contains("@")), 
                Arg.Any<string>(), 
                Arg.Any<string>()
            );
        }

        #endregion
    }
}
