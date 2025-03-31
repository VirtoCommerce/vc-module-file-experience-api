using System;
using VirtoCommerce.FileExperienceApi.Core.Extensions;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.FileExperienceApi.Tests
{
    public class FileExtensionsTests
    {
        [Fact]
        public void SetOwner_WhenImplicitDerivedOwner_ExpectDerivedOwner()
        {
            // Arrange
            var file = new File();
            var owner = new DerivedOwner { Id = "1" };

            // Act
            file.SetOwner(owner);

            // Assert
            Assert.Equal(owner.Id, file.OwnerEntityId);
            Assert.Equal(typeof(DerivedOwner).FullName, file.OwnerEntityType);
        }

        [Fact]
        public void SetOwner_WhenImplicitBaseOwner_ExpectBaseOwner()
        {
            // Arrange
            var file = new File();
            var owner = new BaseOwner { Id = "1" };

            // Act
            file.SetOwner(owner);

            // Assert
            Assert.Equal(owner.Id, file.OwnerEntityId);
            Assert.Equal(typeof(BaseOwner).FullName, file.OwnerEntityType);
        }

        [Fact]
        public void SetOwner_WhenExplicitOwner_ExpectExplicitOwner()
        {
            // Arrange
            var file = new File();
            var owner = new DerivedOwner { Id = "1" };

            // Act
            file.SetOwner<BaseOwner>(owner);

            // Assert
            Assert.Equal(owner.Id, file.OwnerEntityId);
            Assert.Equal(typeof(BaseOwner).FullName, file.OwnerEntityType);
        }

        [Fact]
        public void SetOwner_WhenExplicitOwnerId_ExpectExplicitOwner()
        {
            // Arrange
            const string ownerId = "1";
            var file = new File();

            // Act
            file.SetOwner<BaseOwner>(ownerId);

            // Assert
            Assert.Equal(ownerId, file.OwnerEntityId);
            Assert.Equal(typeof(BaseOwner).FullName, file.OwnerEntityType);
        }

        [Fact]
        public void SetOwner_WhenOwnerIsNull_ExpectArgumentNullException()
        {
            // Arrange
            var file = new File();
            DerivedOwner owner = null;

            // Act
            var action = () =>
            {
                file.SetOwner(owner);
            };

            // Assert
            Assert.Throws<ArgumentNullException>("owner", action);
        }

        [Theory]
        [InlineData("id", "type", false)]
        [InlineData("id", null, false)]
        [InlineData(null, "type", false)]
        [InlineData(null, null, true)]
        public void OwnerIsEmpty(string id, string type, bool expectedIsEmpty)
        {
            // Arrange
            var file = new File { OwnerEntityId = id, OwnerEntityType = type };

            // Act
            var actualIsEmpty = file.OwnerIsEmpty();

            // Assert
            Assert.Equal(expectedIsEmpty, actualIsEmpty);
        }

        [Fact]
        public void OwnerIs_WhenImplicitDerivedOwner_ExpectTrue()
        {
            // Arrange
            var file = new File { OwnerEntityId = "1", OwnerEntityType = typeof(BaseOwner).FullName };
            var owner = new DerivedOwner { Id = "1" };

            // Act
            var expectedResult = file.OwnerIs(owner);

            // Assert
            Assert.True(expectedResult);
        }

        [Fact]
        public void OwnerIs_WhenExplicitDerivedOwner_ExpectTrue()
        {
            // Arrange
            const string ownerId = "1";
            var file = new File { OwnerEntityId = ownerId, OwnerEntityType = typeof(BaseOwner).FullName };

            // Act
            var expectedResult = file.OwnerIs<DerivedOwner>(ownerId);

            // Assert
            Assert.True(expectedResult);
        }

        [Fact]
        public void OwnerTypeIs()
        {
            // Arrange
            var file = new File { OwnerEntityId = "1", OwnerEntityType = typeof(BaseOwner).FullName };

            // Assert
            Assert.True(file.OwnerTypeIs<DerivedOwner>());
            Assert.True(file.OwnerTypeIs<BaseOwner>());
            Assert.False(file.OwnerTypeIs<Entity>());
        }

        [Fact]
        public void ClearOwner_ExpectEmptyOwner()
        {
            // Arrange
            var file = new File { OwnerEntityId = "1", OwnerEntityType = typeof(BaseOwner).FullName };

            // Act
            file.ClearOwner();

            // Assert
            Assert.Null(file.OwnerEntityId);
            Assert.Null(file.OwnerEntityType);
        }

        public class BaseOwner : Entity;

        public class DerivedOwner : BaseOwner;
    }
}
