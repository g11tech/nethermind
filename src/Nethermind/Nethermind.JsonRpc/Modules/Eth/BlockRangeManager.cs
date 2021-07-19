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

using Nethermind.Blockchain.Find;
using Nethermind.Core;
using static Nethermind.JsonRpc.Modules.Eth.EthRpcModule.LastBlockNumberConsts;

namespace Nethermind.JsonRpc.Modules.Eth
{
    public sealed class BlockRangeManager : IBlockRangeManager
    {
        private readonly IBlockFinder _blockFinder;

        public BlockRangeManager(IBlockFinder blockFinder)
        {
            _blockFinder = blockFinder;
        }

        public ResultWrapper<ResolveBlockRangeInfo> ResolveBlockRange(ref long lastBlockNumber, ref long blockCount, int maxHistory)
        {
            Block? pendingBlock = null;
            long? headBlockNumber = null;
            if (lastBlockNumber == PendingBlockNumber)
            {
                pendingBlock = _blockFinder.FindPendingBlock();
                if (pendingBlock != null)
                {
                    lastBlockNumber = pendingBlock.Number;
                    headBlockNumber = pendingBlock.Number - 1;
                }
                else
                {
                    lastBlockNumber = LatestBlockNumber;
                    blockCount--; 
                    if (blockCount == 0)
                    {
                        return ResultWrapper<ResolveBlockRangeInfo>.Fail("Invalid pending block reduced blockCount to 0.");
                    }
                }
            }

            if (headBlockNumber == null)
            {
                headBlockNumber = _blockFinder.FindHeadBlock()?.Number;
                if (headBlockNumber == null)
                {
                    return ResultWrapper<ResolveBlockRangeInfo>.Fail("Head block not found"); //return fail results
                }
            }

            if (lastBlockNumber == LatestBlockNumber)
            {
                lastBlockNumber = (long) headBlockNumber!;
            }
            else if (pendingBlock != null && lastBlockNumber > headBlockNumber)
            {
                return ResultWrapper<ResolveBlockRangeInfo>.Fail("Pending block not present and last block number greater than head number.");
            }
            if (maxHistory != 0)
            {
                long tooOldCount = (long) (headBlockNumber! - maxHistory - lastBlockNumber - blockCount);
                if (blockCount > tooOldCount)
                    blockCount = tooOldCount;
                else
                {
                    return ResultWrapper<ResolveBlockRangeInfo>.Fail("Block count is less than old blocks to remove.");
                }
            }
            if (blockCount > lastBlockNumber + 1)
            {
                blockCount = lastBlockNumber + 1;
            }
            return ResultWrapper<ResolveBlockRangeInfo>.Success(new ResolveBlockRangeInfo(pendingBlock, lastBlockNumber, blockCount));
        }
    }
}
