const path = require("path");
const { spawn } = require("child_process");

const backendPath = path.join(__dirname, "../node-presentation-game-backend");
const frontendPath = path.join(
  __dirname,
  "../vuejs-presentation-game-frontend"
);

const backendProcess = spawn("yarn", ["serve:watch"], {
  cwd: backendPath,
  shell: true
});

backendProcess.stdout.on("data", data => {
  console.log(`backend-stdout: ${data}`);
});

backendProcess.stderr.on("data", data => {
  console.log(`backend-stderr: ${data}`);
});

const frontendProcess = spawn(
  "yarn",
  ["serve", "-p", "8081", "-h", "0.0.0.0"],
  {
    cwd: frontendPath,
    shell: true
  }
);

frontendProcess.stdout.on("data", data => {
  console.log(`frontend-stdout: ${data}`);
});

frontendProcess.stderr.on("data", data => {
  console.log(`frontend-stderr: ${data}`);
});
