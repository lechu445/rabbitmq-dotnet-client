// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 2.0.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2024 Broadcom. All Rights Reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v2.0:
//
//---------------------------------------------------------------------------
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
//  Copyright (c) 2007-2024 Broadcom. All Rights Reserved.
//---------------------------------------------------------------------------

using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Test.Integration
{
    public class TestChannelSoftErrors : IntegrationFixture
    {
        public TestChannelSoftErrors(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestBindOnNonExistingQueue()
        {
            OperationInterruptedException exception = await Assert.ThrowsAsync<OperationInterruptedException>(() => _channel.QueueBindAsync("NonExistingQueue", "", string.Empty));
            Assert.True(exception.Message.Contains("403"), $"Message doesn't contain the expected 403 part: {exception.Message}");
            Assert.False(_channel.IsOpen, "Channel should be closed due to the soft error");
            Assert.True(_conn.IsOpen, "Connection should still be open due to the soft error only closing the channel");
        }

        [Fact]
        public async Task TestBindOnNonExistingExchange()
        {
            OperationInterruptedException exception = await Assert.ThrowsAsync<OperationInterruptedException>(() => _channel.ExchangeBindAsync("NonExistingQueue", "", string.Empty));
            Assert.True(exception.Message.Contains("403"), $"Message doesn't contain the expected 403 part: {exception.Message}");
            Assert.False(_channel.IsOpen, "Channel should be closed due to the soft error");
            Assert.True(_conn.IsOpen, "Connection should still be open due to the soft error only closing the channel");
        }

        [Fact]
        public async Task TestConsumeOnNonExistingQueue()
        {
            OperationInterruptedException exception = await Assert.ThrowsAsync<OperationInterruptedException>(() =>
            {
                var consumer = new EventingBasicConsumer(_channel);
                return _channel.BasicConsumeAsync("NonExistingQueue", true, consumer);
            });

            Assert.True(exception.Message.Contains("404"), $"Message doesn't contain the expected 404 part: {exception.Message}");
            Assert.False(_channel.IsOpen, "Channel should be closed due to the soft error");
            Assert.True(_conn.IsOpen, "Connection should still be open due to the soft error only closing the channel");
        }
    }
}
