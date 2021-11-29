//  Copyright (c) 2021 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
// 

using FluentAssertions;
using Nethermind.AccountAbstraction.Network;
using Nethermind.Core.Extensions;
using NUnit.Framework;

namespace Nethermind.AccountAbstraction.Test.Network
{
    [TestFixture]
    public class HiMessageSerializerTests
    {
        [Test]
        public void Can_do_roundtrip()
        {
            HiMessage hiMessage = new() { ProtocolVersion = 111 };

            HiMessageSerializer serializer = new();
            byte[] serialized = serializer.Serialize(hiMessage);
            byte[] expectedBytes = Bytes.FromHexString("c16f");

            serialized.Should().BeEquivalentTo(expectedBytes);
            
            HiMessage deserialized = serializer.Deserialize(serialized);

            hiMessage.ProtocolVersion.Should().Be(deserialized.ProtocolVersion);
        }
    }
}
