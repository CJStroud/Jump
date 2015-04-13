using Jump;
using Jump.Sprites.Chunks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using NUnit.Framework;
using NUnit.Mocks;

namespace UnitTesting
{
    [TestFixture]
    public class ChunkManagerTests
    {
        [Test]
        public void Add_ValidNormalChunk_AddsChunk()
        {
            var game = Substitute.For<Game>();
            var graphics = Substitute.For<GraphicsDevice>(null, null, new PresentationParameters());
            var texture = Substitute.For<Texture2D>(graphics,0, 0);
            var content = Substitute.For<ContentManager>();
            content.Load<Texture2D>("Building").Returns(texture);

            ChunkManager manager = new ChunkManager(new Rectangle(0,0, 500, 600));
            manager.LoadContent(content, new AudioManager(game));
            manager.Add(new Chunk("Building", Vector2.Zero, 150, 500));

            Assert.AreEqual(1, manager.Chunks.Count);
        }
    }
}
