public class JythonInjection extends HttpServlet {
    protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        response.setContentType("text/plain");
        String code = request.getParameter("code");
        PythonInterpreter interpreter = null;
        ByteArrayOutputStream out = new ByteArrayOutputStream();

        try {
            interpreter = new PythonInterpreter();
            interpreter.setOut(out);
            interpreter.setErr(out);

            // BAD: allow arbitrary Jython expression to execute
            interpreter.exec(code);
            out.flush();

            response.getWriter().print(out.toString());
        } catch(PyException ex) {
            response.getWriter().println(ex.getMessage());
        } finally {
            if (interpreter != null) {
                interpreter.close();
            }
            out.close();
        }
    }

    protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        response.setContentType("text/plain");
        String code = request.getParameter("code");
        PythonInterpreter interpreter = null;

        try {
            interpreter = new PythonInterpreter();
            // BAD: allow arbitrary Jython expression to evaluate
            PyObject py = interpreter.eval(code);

            response.getWriter().print(py.toString());
        } catch(PyException ex) {
            response.getWriter().println(ex.getMessage());
        } finally {
            if (interpreter != null) {
                interpreter.close();
            }
        }
    }
}
