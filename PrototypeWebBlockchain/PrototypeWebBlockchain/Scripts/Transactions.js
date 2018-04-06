web3.eth.defaultAccount = web3.eth.accounts[0];

    
// --Decrypting the block for the transactions
// Requires Abi decoder for NPM

//Declare the Contract Body
var ContractAbi = web3.eth.contract([{ 'constant': false, 'inputs': [{ 'name': '_id', 'type': 'uint256' }, { 'name': '_fileid', 'type': 'uint256' }, { 'name': '_fileHash', 'type': 'string' }, { 'name': '_date', 'type': 'string' }], 'name': 'AddFiles', 'outputs': [], 'payable': false, 'stateMutability': 'nonpayable', 'type': 'function' }, { 'anonymous': false, 'inputs': [{ 'indexed': false, 'name': 'id', 'type': 'uint256' }, { 'indexed': false, 'name': 'fileid', 'type': 'uint256' }, { 'indexed': false, 'name': 'fileHash', 'type': 'string' }, { 'indexed': false, 'name': 'date', 'type': 'string' }], 'name': 'FileUploadEvent', 'type': 'event' }]);

//Declare the decoder abi
const DecoderAbi = [{ 'inputs': [{ 'name': '_id', 'type': 'uint256' }, { 'name': '_fileid', 'type': 'uint256' }, { 'name': '_fileHash', 'type': 'string' }, { 'name': '_date', 'type': 'string' }], 'name': 'AddFiles', 'outputs': [], 'payable': false, 'stateMutability': 'nonpayable', 'type': 'function' }, { 'anonymous': false, 'inputs': [{ 'indexed': false, 'name': 'id', 'type': 'uint256' }, { 'indexed': false, 'name': 'fileid', 'type': 'uint256' }, { 'indexed': false, 'name': 'fileHash', 'type': 'string' }, { 'indexed': false, 'name': 'date', 'type': 'string' }], 'name': 'FileUploadEvent', 'type': 'event' }];

//Add DecoderAbi to the AbiDecoder
abiDecoder.addABI(DecoderAbi);

//Declare the contract Address
var ImageContract = ContractAbi.at('0x538882ec49974f8815fee55ad7b40d6dd4b6b75d');



//Logs decrypted by abi
var datalistdecrypted = [];

var images = [{}];

var count = web3.eth.getTransactionCount("0x198e13017d2333712bd942d8b028610b95c363da");

for (var i = 0; i < web3.eth.getTransactionCount("0x198e13017d2333712bd942d8b028610b95c363da") ; i++) {

    var block = web3.eth.getBlock(web3.eth.blockNumber - i);
    var reciept = web3.eth.getTransactionReceipt(block["transactions"][0]);
    if (reciept["logs"].length != 0)
    {
        datalistdecrypted[i] = abiDecoder.decodeLogs(reciept["logs"]);
        images[i] = datalistdecrypted[i][0]["events"];
    }
}

// --End of Decryption

//Dynamic Approach 

//for (var image = 0; image < images.length ; image++) {
//    $("#transactiontable").append("<tr id='tr" + image + "'>");

//    for (var object = 0; object < images[0].length; object++)
//    {
//        $("#" + "tr" + image).append('<td>' + images[image][object]["value"] + '</td>')
//        var test = images[image][object]["value"];
//    }

//    $("#transactiontable").append('</tr>');
//}

//Static Approach

var jsonImage = new Array();

for (var image = 0; image < images.length ; image++) {
    if (images[image][0]["value"] == $("#id").val())    
    {
        jsonImage.push(new Object({
            id: images[image][0]["value"],
            fileid: images[image][1]["value"],
            filehash: images[image][2]["value"],
            date: images[image][3]["value"]
        }));
    }
}

var data = JSON.stringify(jsonImage);

$.ajax({
    url: "ValidateImages",
    type: "POST",
    data: { "data":  JSON.stringify(jsonImage) },
    success: function (ImageFiles) {
        if (ImageFiles)

            for (var i = 0; i < ImageFiles["ImageFiles"].length; i++) {
                
                $("#transactiontable").append("<tr><td>" + ImageFiles["ImageFiles"][i]["id"] + "</td><td>" + ImageFiles["ImageFiles"][i]["fileid"] + "</td> <td>  <img src=/images/" + ImageFiles["ImageFiles"][i]["filehash"] + " style='width:250px; height:250px'> </td> <td>" + ImageFiles["ImageFiles"][i]["date"] + "</td> </tr>");

            }
     
}
});