
const currencyKeyName = 'coins'

const rpcAddUserCoins: nkruntime.RpcFunction = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
    let walletUpdateResult = updateWallet(nk, ctx.userId, 100, {});
    let updateString = JSON.stringify(walletUpdateResult);

    logger.debug('Added 100 coins to user %s wallet: %s', ctx.userId, updateString);

    return updateString;
}

function updateWallet(nk: nkruntime.Nakama, userId: string, amount: number, metadata: {[key: string]: any}): nkruntime.WalletUpdateResult {
    const changeset = {
        [currencyKeyName]: amount,
    }
    let result = nk.walletUpdate(userId, changeset, metadata, true);

    return result;
}
