const vscode = require('vscode');

/**
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {
	vscode.window.showInformationMessage('Initializing NewLang...');

	const disposable = vscode.commands.registerCommand('nlangdevkit.helloWorld', function () {
		vscode.window.showInformationMessage('Hello World from NewLang!');
	});

	context.subscriptions.push(disposable);
}

// This method is called when your extension is deactivated
function deactivate() {
	// nothing to do
}

module.exports = {
	activate,
	deactivate
};
