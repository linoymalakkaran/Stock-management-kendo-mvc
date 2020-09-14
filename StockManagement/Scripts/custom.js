
function printContent() {
    var DocumentContainer = document.getElementById("grid");
    var WindowObject = window.open("", "PrintWindow", "width=1200,height=650,top=50,left=50,toolbars=no,scrollbars=yes,status=no,resizable=yes");
    WindowObject.document.write('<html><head><title>Category List Printing</title>');
    WindowObject.document.writeln('<link rel="stylesheet" type="text/css" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css" />');
    WindowObject.document.write('</head><body>');
    WindowObject.document.writeln(DocumentContainer.innerHTML);
    WindowObject.document.write('</body></html>');
    WindowObject.document.close();
    WindowObject.focus();
    WindowObject.print();
    WindowObject.close();
}