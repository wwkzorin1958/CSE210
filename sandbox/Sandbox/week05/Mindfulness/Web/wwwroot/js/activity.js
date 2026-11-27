(() => {
    const data = document.querySelector("#app-data").dataset;
    const prompts = {
        listing: JSON.parse(data.listing),
        reflection: JSON.parse(data.reflection),
        questions: JSON.parse(data.questions)
    };
    const home = document.querySelector("#home-screen");
    const activities = document.querySelector("#activities");
    const screen = document.querySelector("#activity-screen");
    const kindLabel = document.querySelector("#session-kind");
    const timeLabel = document.querySelector("#session-time");
    const title = document.querySelector("#session-title");
    const description = document.querySelector("#session-description");
    const prompt = document.querySelector("#session-prompt");
    const responseLabel = document.querySelector("#response-label");
    const input = document.querySelector("#response");
    const items = document.querySelector("#session-items");
    const error = document.querySelector("#session-error");
    const submit = document.querySelector("#submit-response");
    const nextPrompt = document.querySelector("#next-prompt");
    const finishButton = document.querySelector("#finish-session");
    const cancel = document.querySelector("#cancel-session");
    const durationInput = document.querySelector("#duration");
    const token = document.querySelector("#antiforgery-form input[name='__RequestVerificationToken']").value;
    const sounds = [new Audio("/sounds/Breathin.wav"), new Audio("/sounds/Breathout.wav")];
    let session = null;
    let interval = null;

    function choose(values) {
        return values[Math.floor(Math.random() * values.length)];
    }

    function playSound(high) {
        const sound = sounds[high ? 1 : 0];
        sound.currentTime = 0;
        sound.play().catch(() => {
            error.textContent = "The bowl sound could not play. Check your browser's audio settings.";
        });
    }

    function setControls(showInput, submitText, showNext, showFinish) {
        responseLabel.hidden = !showInput;
        input.hidden = !showInput;
        submit.hidden = !submitText;
        submit.textContent = submitText || "";
        nextPrompt.hidden = !showNext;
        finishButton.hidden = !showFinish;
        if (showInput) input.focus();
    }

    function begin(kind) {
        const duration = Number(durationInput.value);
        if (!Number.isInteger(duration) || duration < 1 || duration > 86400) {
            durationInput.setCustomValidity("Choose a whole number between 1 and 86400 seconds.");
            durationInput.reportValidity();
            return;
        }
        durationInput.setCustomValidity("");
        session = {
            kind, duration, phase: "preparing", readyAt: performance.now(),
            responses: [], count: 0, questions: [], highTone: true,
            requestId: crypto.randomUUID(), pending: null
        };
        home.hidden = true;
        activities.hidden = true;
        screen.hidden = false;
        kindLabel.textContent = `${kind.toUpperCase()} / GET READY`;
        title.textContent = "Take a breath.";
        description.textContent = "Your practice is about to begin.";
        prompt.textContent = "";
        items.textContent = "";
        error.textContent = "";
        cancel.textContent = "Cancel session";
        setControls(false, "", false, false);
        playSound(false);
        interval = window.setInterval(tick, 200);
        tick();
    }

    function beginTimed() {
        session.startedAt = new Date().toISOString();
        session.started = performance.now();
    }

    function beginActivity() {
        kindLabel.textContent = `${session.kind.toUpperCase()} / YOUR PRACTICE`;
        if (session.kind === "Breathing") {
            session.phase = "breathing";
            session.lastPhase = 0;
            beginTimed();
            title.textContent = "Breathe in.";
            description.textContent = "Follow the gentle rhythm. Complete each breath at your own pace.";
            prompt.textContent = "5";
            playSound(false);
        } else if (session.kind === "Listing") {
            session.phase = "listing";
            session.currentPrompt = choose(prompts.listing);
            session.responses.push({ Prompt: session.currentPrompt, Answers: [] });
            beginTimed();
            showListing();
            playSound(false);
        } else {
            session.phase = "reflect-intro";
            timeLabel.textContent = "TAKE YOUR TIME";
            session.currentPrompt = choose(prompts.reflection);
            title.textContent = "A moment to reflect.";
            description.textContent = "Bring an experience to mind. Your timed questions begin when you continue.";
            prompt.textContent = session.currentPrompt;
            input.value = "";
            setControls(true, "Continue →", false, false);
            playSound(false);
        }
        tick();
    }

    function tick() {
        if (!session) return;
        if (session.phase === "preparing") {
            const remaining = Math.max(0, 5 - Math.floor((performance.now() - session.readyAt) / 1000));
            timeLabel.textContent = `STARTING IN ${remaining}`;
            if (remaining === 0) beginActivity();
            return;
        }
        if (!session.started || session.phase === "reflect-intro" ||
            session.phase === "saving" || session.phase === "complete") return;
        const elapsed = (performance.now() - session.started) / 1000;
        const remaining = Math.max(0, Math.ceil(session.duration - elapsed));
        timeLabel.textContent = remaining > 0 ? `${remaining} SEC LEFT` : "TIME IS UP";
        if (session.phase === "breathing") {
            if (elapsed >= Math.ceil(session.duration / 10) * 10) {
                finish();
                return;
            }
            const phase = Math.floor(elapsed / 5);
            if (phase !== session.lastPhase) {
                session.lastPhase = phase;
                playSound(phase % 2 === 1);
            }
            title.textContent = phase % 2 === 0 ? "Breathe in." : "Breathe out.";
            prompt.textContent = String(5 - Math.floor(elapsed % 5));
            if (remaining === 0) timeLabel.textContent = "FINISHING YOUR BREATH";
        } else if (remaining === 0) {
            description.textContent = "Time is up. Submit your current response to finish.";
        }
    }

    function showListing() {
        title.textContent = "Notice what matters.";
        description.textContent = "Enter one item at a time. Leave the field blank for another prompt.";
        prompt.textContent = session.currentPrompt;
        items.textContent = `Items listed: ${session.count}`;
        input.value = "";
        setControls(true, "Add item →", true, true);
    }

    function switchListingPrompt() {
        const alternatives = prompts.listing.filter(value => value !== session.currentPrompt);
        if (alternatives.length === 0) {
            error.textContent = "There are no other prompts available.";
            return;
        }
        if (session.responses.length >= 500) {
            error.textContent = "The prompt limit is reached. Finish and save this session.";
            return;
        }
        session.currentPrompt = choose(alternatives);
        session.responses.push({ Prompt: session.currentPrompt, Answers: [] });
        showListing();
    }

    function question() {
        if (session.questions.length === 0) session.questions = [...prompts.questions];
        const index = Math.floor(Math.random() * session.questions.length);
        session.currentPrompt = session.questions.splice(index, 1)[0];
        title.textContent = "Go a little deeper.";
        description.textContent = "Write a response, or leave it blank to skip.";
        prompt.textContent = session.currentPrompt;
        input.value = "";
        setControls(true, "Next question →", false, true);
        playSound(session.highTone);
        session.highTone = !session.highTone;
    }

    function addCurrentResponse() {
        if (session.phase === "listing") {
            if (input.value.trim()) {
                if (session.count >= 500) {
                    error.textContent = "The item limit is reached. Finish and save this session.";
                    return false;
                }
                session.responses[session.responses.length - 1].Answers.push(input.value);
                session.count++;
            }
        } else if (session.phase === "reflecting") {
            if (session.responses.length >= 500) {
                error.textContent = "The question limit is reached. Finish and save this session.";
                return false;
            }
            session.responses.push({ Prompt: session.currentPrompt, Answers: [input.value] });
        }
        return true;
    }

    function submitResponse() {
        if (session.phase === "saving") {
            save();
            return;
        }
        if (session.phase === "reflect-intro") {
            session.responses.push({ Prompt: session.currentPrompt, Answers: [input.value] });
            session.phase = "reflecting";
            beginTimed();
            question();
            tick();
            return;
        }
        if (session.phase !== "listing" && session.phase !== "reflecting") return;
        const wasBlank = !input.value.trim();
        if (!addCurrentResponse()) return;
        if ((performance.now() - session.started) / 1000 >= session.duration ||
            (session.phase === "listing" && session.count >= 500) ||
            (session.phase === "reflecting" && session.responses.length >= 500)) {
            if (session.phase === "listing") playSound(true);
            finish();
        } else if (session.phase === "listing") {
            if (wasBlank) switchListingPrompt();
            else showListing();
        } else {
            question();
        }
    }

    function finish() {
        window.clearInterval(interval);
        session.pending = {
            RequestId: session.requestId,
            ActivityName: session.kind,
            StartedAt: session.startedAt,
            RequestedSeconds: session.duration,
            ElapsedSeconds: (performance.now() - session.started) / 1000,
            Responses: session.responses
        };
        session.phase = "saving";
        save();
    }

    async function save() {
        submit.disabled = true;
        cancel.disabled = true;
        title.textContent = "Saving your moment.";
        description.textContent = "Please wait.";
        error.textContent = "";
        prompt.textContent = "";
        setControls(false, "", false, false);
        try {
            const result = await fetch("/?handler=Save", {
                method: "POST",
                headers: { "Content-Type": "application/json", "RequestVerificationToken": token },
                body: JSON.stringify(session.pending)
            });
            if (!result.ok || result.redirected) {
                throw new Error(result.redirected
                    ? "Your session has expired. Sign in in another tab, then retry."
                    : `The server returned ${result.status}.`);
            }
            session.phase = "complete";
            kindLabel.textContent = `${session.kind.toUpperCase()} / COMPLETE`;
            timeLabel.textContent = "WELL DONE";
            title.textContent = "You made space.";
            description.textContent = session.kind === "Listing"
                ? `You listed ${session.count} items. Your session has been saved.`
                : "Your session has been saved to your history.";
            items.textContent = "";
            cancel.textContent = "Return to activities";
        } catch (problem) {
            title.textContent = "Your session isn't saved yet.";
            description.textContent = "Your responses are still in this window. Fix the issue and retry.";
            error.textContent = problem instanceof Error ? problem.message : "Unable to save the session.";
            setControls(false, "Retry saving →", false, false);
        } finally {
            submit.disabled = false;
            cancel.disabled = false;
        }
    }

    document.querySelectorAll("[data-activity]").forEach(button => {
        button.addEventListener("click", () => begin(button.dataset.activity));
    });
    durationInput.addEventListener("input", () => durationInput.setCustomValidity(""));
    submit.addEventListener("click", submitResponse);
    input.addEventListener("keydown", event => {
        if (event.key === "Enter") {
            event.preventDefault();
            submitResponse();
        }
    });
    nextPrompt.addEventListener("click", () => {
        if ((performance.now() - session.started) / 1000 >= session.duration) {
            submitResponse();
        } else if (input.value.trim()) {
            error.textContent = "Add the current item before changing prompts.";
        } else {
            switchListingPrompt();
        }
    });
    finishButton.addEventListener("click", () => {
        if (addCurrentResponse()) {
            if (session.phase === "listing") playSound(true);
            finish();
        }
    });
    cancel.addEventListener("click", () => {
        if (session.phase !== "complete" && !window.confirm("Leave without saving this session?")) return;
        window.clearInterval(interval);
        session = null;
        screen.hidden = true;
        home.hidden = false;
        activities.hidden = false;
    });
    window.addEventListener("beforeunload", event => {
        if (session && session.phase !== "complete") {
            event.preventDefault();
        }
    });
})();
